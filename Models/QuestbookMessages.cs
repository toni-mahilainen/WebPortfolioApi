using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class QuestbookMessages
    {
        public int MessageId { get; set; }
        public int PortfolioId { get; set; }
        public int VisitorId { get; set; }
        public string Message { get; set; }
        public DateTime VisitationTimestamp { get; set; }

        public virtual PortfolioContent Portfolio { get; set; }
        public virtual Visitors Visitor { get; set; }
    }
}
