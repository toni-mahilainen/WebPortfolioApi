using System;
using System.Collections.Generic;

namespace WebPortfolioCoreApi.Models
{
    public partial class ImageTypes
    {
        public ImageTypes()
        {
            ImageUrls = new HashSet<ImageUrls>();
        }

        public int TypeId { get; set; }
        public string Type { get; set; }

        public virtual ICollection<ImageUrls> ImageUrls { get; set; }
    }
}
