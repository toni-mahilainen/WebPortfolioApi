using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class SocialMediaLinks
    {
        public int LinkId { get; set; }
        public int PortfolioId { get; set; }
        public int ServiceId { get; set; }
        public string Link { get; set; }

        public virtual PortfolioContent Portfolio { get; set; }
        public virtual SocialMediaServices Service { get; set; }
    }
}
