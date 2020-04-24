using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class ImageUrls
    {
        public int UrlId { get; set; }
        public int UserId { get; set; }
        public int TypeId { get; set; }
        public string Url { get; set; }

        public virtual ImageTypes Type { get; set; }
        public virtual Users User { get; set; }
    }
}
