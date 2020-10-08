using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPortfolioCoreApi.OtherModels
{
    public class Passwords
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ResetToken { get; set; }
    }
}
