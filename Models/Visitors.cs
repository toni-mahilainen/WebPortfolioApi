using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class Visitors
    {
        public Visitors()
        {
            QuestbookMessages = new HashSet<QuestbookMessages>();
        }

        public int VisitorId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Company { get; set; }

        public virtual ICollection<QuestbookMessages> QuestbookMessages { get; set; }
    }
}
