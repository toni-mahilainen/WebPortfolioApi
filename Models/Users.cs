using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class Users
    {
        public Users()
        {
            PortfolioContent = new HashSet<PortfolioContent>();
            Skills = new HashSet<Skills>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<PortfolioContent> PortfolioContent { get; set; }
        public virtual ICollection<Skills> Skills { get; set; }
    }
}
