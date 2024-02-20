using Kikolo_v1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;

namespace Kikolo_v1.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache;
        private readonly KikoloContext _context;
        private static List<int> displayedQuestionIds = new List<int>();
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _openAiApiKey = "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";

        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache, KikoloContext context, IHttpClientFactory clientFactory)
        {
            _cache = memoryCache;
            _context = context;
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static int roundCount = -1;

        public IActionResult Game()
        {
            var selectedFragenpackId = HttpContext.Session.GetInt32("SelectedFragenpackId");
            if (selectedFragenpackId == null)
            {
                // Kein Fragenpack ausgewählt, leiten Sie zu einer geeigneten Seite um
                return RedirectToAction("SomeOtherAction");
            }

            roundCount++;
            var playerListe = GetPlayerListe();
            var frage = GetRandomQuestionBasedOnOdds();

            while (frage != null && displayedQuestionIds.Contains(frage.Id))
            {
                frage = GetRandomQuestionBasedOnOdds();
            }

            if (frage != null)
            {
                displayedQuestionIds.Add(frage.Id); // Schon benutzte Fragen 

                if (playerListe.Any())
                {
                    var random = new Random();
                    var randomPlayer = playerListe[random.Next(playerListe.Count)];
                    frage.Text = frage.Text.Replace("{randomPlayer}", randomPlayer);
                }
            }

            if (roundCount == 10)
            {
                roundCount = 0;
                displayedQuestionIds.Clear();
                ClearGameSessions(); 
                return RedirectToAction("GameOver");
            }

            return View(new List<Question> { frage });
        }

        private void ClearGameSessions()
        {
            HttpContext.Session.Remove("playerListe");
            HttpContext.Session.Remove("SelectedFragenpackId");
        }
        public IActionResult Auswahl()
        {
            return View();
        }

        private List<string> GetPlayerListe()
        {
            List<string> playerListe = new List<string>();

            if (HttpContext.Session.Get("playerListe") is byte[] data)
            {
                playerListe = DeserializeFromByteArray<List<string>>(data);
            }

            return playerListe;
        }

        private Question GetRandomQuestionBasedOnOdds()
        {
            var selectedFragenpackId = HttpContext.Session.GetInt32("SelectedFragenpackId");
            if (selectedFragenpackId == null)
            {
                // Kein Fragenpack ausgewählt, geben Sie null zurück oder leiten Sie um
                return null; // Oder eine andere geeignete Behandlung
            }

            var random = new Random();
            var randomNumber = random.Next(100);
            var category = randomNumber < 70 ? "Question" : randomNumber < 90 ? "Voting" : "Minigame";

            return _context.Questions
                           .Where(q => q.Category == category && q.Fragenpacks.Any(fp => fp.Id == selectedFragenpackId.Value))
                           .OrderBy(q => Guid.NewGuid())
                           .FirstOrDefault();
        }

        public IActionResult Spieler()
        {
            List<string> playerListe;

            if (HttpContext.Session.Get("playerListe") is byte[] data)
            {
                playerListe = DeserializeFromByteArray<List<string>>(data);
            }
            else
            {
                playerListe = new List<string>();
            }

            var fragenpacks = _context.Fragenpacks.ToList(); // Laden der Fragenpacks
            ViewBag.PlayerList = playerListe;
            ViewBag.Fragenpacks = new SelectList(fragenpacks, "Id", "Name"); // Übergeben an die View

            return View(playerListe);
        }

        [HttpPost]
        public IActionResult Spieler(string spielerName)
        {
            List<string> playerListe;

            if (HttpContext.Session.Get("playerListe") is byte[] data)
            {
                playerListe = DeserializeFromByteArray<List<string>>(data);
            }
            else
            {
                playerListe = new List<string>();
            }

            if (!string.IsNullOrEmpty(spielerName))
            {
                playerListe.Add(spielerName);
            }

            ViewBag.PlayerList = playerListe;
            HttpContext.Session.Set("playerListe", SerializeToByteArray(playerListe));

            var fragenpacks = _context.Fragenpacks.ToList(); // Laden der Fragenpacks für die Post-Anfrage
            ViewBag.Fragenpacks = new SelectList(fragenpacks, "Id", "Name");

            return View(playerListe);
        }

        [HttpPost]
        public IActionResult SetFragenpack(int fragenpackId)
        {
            HttpContext.Session.SetInt32("SelectedFragenpackId", fragenpackId);
            return RedirectToAction("Spieler");
        }

        public async Task<IActionResult> Admin()
        {
            var fragenListe = await _context.Questions
                                            .Include(q => q.Fragenpacks) 
                                            .ToListAsync();
            var fragenpacks = await _context.Fragenpacks.ToListAsync();
            ViewBag.Fragenpacks = new SelectList(fragenpacks, "ID", "Name");

            return View(fragenListe);
        }

        [HttpPost]
        public async Task<IActionResult> Admin(string questionText, string category, int fragenpackId)
        {
            if (!string.IsNullOrEmpty(questionText))
            {
                var neueFrage = new Question
                {
                    Text = questionText,
                    Category = category,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Questions.Add(neueFrage);
                await _context.SaveChangesAsync();

                if (fragenpackId > 0)
                {
                    var fragenpack = await _context.Fragenpacks.FindAsync(fragenpackId);
                    if (fragenpack != null)
                    {
                        // Fügen Sie die Frage zum Fragenpack hinzu
                        fragenpack.Questions.Add(neueFrage);
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction("Admin");
            }
         
            ViewBag.Fragenpacks = new SelectList(await _context.Fragenpacks.ToListAsync(), "ID", "Name");
            return View("Admin", await _context.Questions.ToListAsync());
        }

        private byte[] SerializeToByteArray<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        private T DeserializeFromByteArray<T>(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(json);
        }
        public IActionResult GameOver()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeleteSpieler(string spielerName)
        {
            var playerListe = GetPlayerListe();

            if (!string.IsNullOrEmpty(spielerName) && playerListe.Contains(spielerName))
            {
                playerListe.Remove(spielerName);
                HttpContext.Session.Set("playerListe", SerializeToByteArray(playerListe));
            }

            return RedirectToAction("Spieler");
        }

        public IActionResult DeleteConfirm(int id)
        {
            var frage = _context.Questions.Find(id);
            if (frage == null)
            {
                return NotFound();
            }
            return View(frage);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var questionToDelete = _context.Questions.Find(id);
            if (questionToDelete != null)
            {
                _context.Questions.Remove(questionToDelete);
                _context.SaveChanges();
            }

            return RedirectToAction("Admin");
        }

        public IActionResult Edit(int id)
        {
            var frage = _context.Questions.Find(id);
            if (frage == null)
            {
                return NotFound();
            }
            return View(frage);
        }

        [HttpPost]
        public IActionResult Edit(Question editedQuestion)
        {
            if (ModelState.IsValid)
            {
                _context.Questions.Update(editedQuestion);
                _context.SaveChanges();
                return RedirectToAction("Admin");
            }
            return View("Admin", editedQuestion);
        }

        public IActionResult FragenPacks()
        {
            var fragenpacks = _context.Fragenpacks.ToList(); 

            return View(fragenpacks);
        }

        public async Task<IActionResult> SelectedFragenpack(int id)
        {
            var fragenpack = await _context.Fragenpacks
                                .Include(fp => fp.Questions)
                                .FirstOrDefaultAsync(fp => fp.Id == id);

            if (fragenpack == null)
            {
                return NotFound();
            }
            ViewBag.FragenpackName = fragenpack.Name;

            return View(fragenpack.Questions.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SavePlayerDescription(string spielerName, string beschreibung)
        {
            if (string.IsNullOrEmpty(spielerName) || string.IsNullOrEmpty(beschreibung))
            {
                return BadRequest("Spielername oder Beschreibung fehlt.");
            }

            List<string> playerListe = GetPlayerListe();
            Dictionary<string, string> spielerDaten = new Dictionary<string, string>();

            if (HttpContext.Session.Get("spielerDaten") is byte[] spielerDatenBytes)
            {
                spielerDaten = DeserializeFromByteArray<Dictionary<string, string>>(spielerDatenBytes);
            }

            spielerDaten[spielerName] = beschreibung;

            HttpContext.Session.Set("spielerDaten", SerializeToByteArray(spielerDaten));

            return Ok("Spielerbeschreibung erfolgreich gespeichert.");

            //API-Key: 
        }

        private Dictionary<string, string> GetPlayerDescriptions()
        {
            var spielerDatenBytes = HttpContext.Session.Get("spielerDaten");
            if (spielerDatenBytes != null)
            {
                var json = Encoding.UTF8.GetString(spielerDatenBytes);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            return new Dictionary<string, string>();
        }

        public async Task<IActionResult> GenerateQuestionFromDescriptions()
        {
            var spielerDaten = GetPlayerDescriptions();
            var beschreibungen = spielerDaten.Values.ToList();
            if (!beschreibungen.Any())
            {
                return BadRequest("Keine Spielerbeschreibungen verfügbar.");
            }

            // Erstellen des Prompts für die Chat-API
            List<dynamic> messages = beschreibungen.Select(desc => new { role = "user", content = desc }).ToList<dynamic>();
            messages.Insert(0, new { role = "system", content = "Ich organisiere ein Trinkspiel für eine Gruppe von Freunden, die vielfältige Interessen haben, diese werde ich dir folgend mitteilen. Um das Spiel unterhaltsam und interaktiv zu gestalten, benötige ich deine Hilfe, um Fragen und Aktivitäten in drei Kategorien zu generieren:\r\n\r\n1. Normale Fragen: Allgemeine oder spezifische Fragen, die eine offene Antwort erfordern und zur Diskussion anregen.\r\n2. Voting: Entweder-oder-Fragen, die die Teilnehmer abstimmen lassen, um Unterschiede in Meinungen oder Vorlieben aufzudecken.\r\n3. Minigames: Kurze und unterhaltsame Spiele, die leicht zu erklären und durchzuführen sind, ähnlich wie Activity, die Kreativität und Interaktion fördern.\r\n\r\nBitte erstelle für jede Kategorie fünf Vorschläge, die zu einer fröhlichen und engagierten Stimmung beitragen, vor jeder frage soll die kategorie dabei stehen. Der Fokus sollte auf Spaß, Einbindung aller Teilnehmer und der Förderung von Gesprächen liegen." });

            var httpClient = _clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

            var requestContent = new
            {
                model = "gpt-3.5-turbo", 
                messages,
                temperature = 0.7,
                max_tokens = 500,
                top_p = 1.0,
                frequency_penalty = 0.0,
                presence_penalty = 0.0,
                n = 1 // Anzahl der generierten Antworten, anpassen falls erforderlich
            };

            var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<dynamic>(responseString);

                var questions = new List<string>();
                foreach (var choice in responseObject.choices)
                {
                    questions.Add((string)choice.message.content); 
                }

                Console.WriteLine("questions" + questions);
                return Json(questions);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return BadRequest($"Fehler bei der Anfrage an OpenAI: {errorContent}");
            }

            //var errorContent = await response.Content.ReadAsStringAsync();
            //return BadRequest($"Fehler bei der Anfrage an OpenAI: {errorContent}");
        }
    }
}
