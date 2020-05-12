using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class Users
    {
        public Users()
        {
            ImageUrls = new HashSet<ImageUrls>();
            PortfolioContent = new HashSet<PortfolioContent>();
            Skills = new HashSet<Skills>();
            SocialMediaLinks = new HashSet<SocialMediaLinks>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }

        public virtual ICollection<ImageUrls> ImageUrls { get; set; }
        public virtual ICollection<PortfolioContent> PortfolioContent { get; set; }
        public virtual ICollection<Skills> Skills { get; set; }
        public virtual ICollection<SocialMediaLinks> SocialMediaLinks { get; set; }
    }
}
