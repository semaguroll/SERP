using CommandLine;
using Google.Ads.Gax.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERP1
{
    public class Options : OptionsBase
    {

        [Option("customerId", Required = true)]
        public long CustomerId { get; set; }

       
        [Option("locationIds", Required = true)]
        public IEnumerable<long> LocationIds { get; set; }

       
        [Option("languageId", Required = true)]
        public long LanguageId { get; set; }

        
        [Option("keywordTexts", Required = false, Default = new string[] { })]
        public IEnumerable<string> KeywordTexts { get; set; }

        
        [Option("pageUrl", Required = false)]
        public string PageUrl { get; set; }
    

    }
}
