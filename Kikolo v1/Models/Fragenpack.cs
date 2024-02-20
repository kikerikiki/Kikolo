using System;
using System.Collections.Generic;

namespace Kikolo_v1.Models
{
    public partial class Fragenpack
    {
        public Fragenpack()
        {
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
