using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class Projects
    {
        public int ProjectId { get; set; }
        public int SkillId { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }

        public virtual Skills Skill { get; set; }
    }
}
