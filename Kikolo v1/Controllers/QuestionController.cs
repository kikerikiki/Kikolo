﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Kikolo_v1.Models;

//namespace Kikolo_v1.Controllers
//{
//    public class QuestionController : Controller
//    {
//        private readonly KikoloContext _context;

//        public QuestionController(KikoloContext context)
//        {
//            _context = context;
//        }

//        // GET: Question
//        public async Task<IActionResult> Index()
//        {
//              return _context.Questions != null ? 
//                          View(await _context.Questions.ToListAsync()) :
//                          Problem("Entity set 'KikoloContext.Questions'  is null.");
//        }

//        // GET: Question/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null || _context.Questions == null)
//            {
//                return NotFound();
//            }

//            var question = await _context.Questions
//                .FirstOrDefaultAsync(m => m.Id == id);
//            if (question == null)
//            {
//                return NotFound();
//            }

//            return View(question);
//        }

//        // GET: Question/Create
//        public IActionResult Create()
//        {
//            return View();
//        }

//        // POST: Question/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("Id,Text,Category,CreatedAt")] Question question)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(question);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(question);
//        }

//        // GET: Question/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null || _context.Questions == null)
//            {
//                return NotFound();
//            }

//            var question = await _context.Questions.FindAsync(id);
//            if (question == null)
//            {
//                return NotFound();
//            }
//            return View(question);
//        }

//        // POST: Question/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("Id,Text,Category,CreatedAt")] Question question)
//        {
//            if (id != question.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(question);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!QuestionExists(question.Id))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            return View(question);
//        }

//        // GET: Question/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null || _context.Questions == null)
//            {
//                return NotFound();
//            }

//            var question = await _context.Questions
//                .FirstOrDefaultAsync(m => m.Id == id);
//            if (question == null)
//            {
//                return NotFound();
//            }

//            return View(question);
//        }

//        // POST: Question/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            if (_context.Questions == null)
//            {
//                return Problem("Entity set 'KikoloContext.Questions'  is null.");
//            }
//            var question = await _context.Questions.FindAsync(id);
//            if (question != null)
//            {
//                _context.Questions.Remove(question);
//            }
            
//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool QuestionExists(int id)
//        {
//          return (_context.Questions?.Any(e => e.Id == id)).GetValueOrDefault();
//        }
//    }
//}
