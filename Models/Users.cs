using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class Users
    {
        public Users()
        {
            Emails = new HashSet<Emails>();
            ImageUrls = new HashSet<ImageUrls>();
            PortfolioContent = new HashSet<PortfolioContent>();
            QuestbookMessages = new HashSet<QuestbookMessages>();
            Skills = new HashSet<Skills>();
            SocialMediaLinks = new HashSet<SocialMediaLinks>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string JwtToken { get; set; }
        public string SasToken { get; set; }
        public string PasswordResetToken { get; set; }
        public int? ThemeId { get; set; }

        public virtual Themes Theme { get; set; }
        public virtual ICollection<Emails> Emails { get; set; }
        public virtual ICollection<ImageUrls> ImageUrls { get; set; }
        public virtual ICollection<PortfolioContent> PortfolioContent { get; set; }
        public virtual ICollection<QuestbookMessages> QuestbookMessages { get; set; }
        public virtual ICollection<Skills> Skills { get; set; }
        public virtual ICollection<SocialMediaLinks> SocialMediaLinks { get; set; }
    }
}
