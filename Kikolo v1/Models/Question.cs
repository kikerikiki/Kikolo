using System;
using System.Collections.Generic;

namespace Kikolo_v1.Models
{
    public partial class Question
    {
        public Question()
        {
            Fragenpacks = new HashSet<Fragenpack>();
        }

        public int Id { get; set; }
        public string? Text { get; set; }
        public string? Category { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Fragenpack> Fragenpacks { get; set; }
    }
}
