using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPortfolioCoreApi.OtherModels
{
    public class AllContent
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime Birthdate { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string[] Emails { get; set; }
        public string Phonenumber { get; set; }
        public string Punchline { get; set; }
        public string BasicKnowledge { get; set; }
        public string Education { get; set; }
        public string WorkHistory { get; set; }
        public string LanguageSkills { get; set; }
    }
}
