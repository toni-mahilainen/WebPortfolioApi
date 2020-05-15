using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class QuestbookMessages
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public int VisitorId { get; set; }
        public string Message { get; set; }
        public DateTime VisitationTimestamp { get; set; }

        public virtual Users User { get; set; }
        public virtual Visitors Visitor { get; set; }
    }
}
