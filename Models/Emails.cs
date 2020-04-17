using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class Emails
    {
        public int EmailId { get; set; }
        public int PortfolioId { get; set; }
        public string EmailAddress { get; set; }

        public virtual PortfolioContent Portfolio { get; set; }
    }
}
