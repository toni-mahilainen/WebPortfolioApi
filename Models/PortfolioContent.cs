using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class PortfolioContent
    {
        public PortfolioContent()
        {
            Emails = new HashSet<Emails>();
            QuestbookMessages = new HashSet<QuestbookMessages>();
        }

        public int PortfolioId { get; set; }
        public int UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime? Birthdate { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phonenumber { get; set; }
        public string Punchline { get; set; }
        public string BasicKnowledge { get; set; }
        public string Education { get; set; }
        public string WorkHistory { get; set; }
        public string LanguageSkills { get; set; }

        public virtual Users User { get; set; }
        public virtual ICollection<Emails> Emails { get; set; }
        public virtual ICollection<QuestbookMessages> QuestbookMessages { get; set; }
    }
}
