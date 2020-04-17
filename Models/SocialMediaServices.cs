using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class SocialMediaServices
    {
        public SocialMediaServices()
        {
            SocialMediaLinks = new HashSet<SocialMediaLinks>();
        }

        public int ServiceId { get; set; }
        public string Service { get; set; }

        public virtual ICollection<SocialMediaLinks> SocialMediaLinks { get; set; }
    }
}
