using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPortfolioCoreApi.Models
{
    public class NewMessage
    {
        public string VisitorFirstname { get; set; }

        public string VisitorLastname { get; set; }

        public string VisitorCompany { get; set; }

        public string Message { get; set; }

        public DateTime VisitationTimestamp { get; set; }
    }
}
