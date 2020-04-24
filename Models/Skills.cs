using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class Skills
    {
        public Skills()
        {
            Projects = new HashSet<Projects>();
        }

        public int SkillId { get; set; }
        public int UserId { get; set; }
        public string Skill { get; set; }
        public int SkillLevel { get; set; }

        public virtual Users User { get; set; }
        public virtual ICollection<Projects> Projects { get; set; }
    }
}
