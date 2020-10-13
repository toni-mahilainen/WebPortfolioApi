using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPortfolioCoreApi
{
    public class Secrets
    {
        public static string AzureAccessKey { get; set; }
        public string SendingEmail { get; set; }
        public string EmailPassword { get; set; }
    }
}
