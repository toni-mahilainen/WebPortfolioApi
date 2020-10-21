using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class Themes
    {
        public Themes()
        {
            Users = new HashSet<Users>();
        }

        public int ThemeId { get; set; }
        public string ThemeName { get; set; }

        public virtual ICollection<Users> Users { get; set; }
    }
}
