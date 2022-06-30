using Google.Ads.Gax.Examples;
using Google.Ads.GoogleAds.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERP1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Options options = new Options();

           
            options.KeywordTexts = new string[] { "Bilgisayar" };
            options.CustomerId = long.Parse("7403111303");
            options.LocationIds = new long[] { long.Parse("21069") };
            options.PageUrl = "https://medium.com/";
            options.LanguageId = long.Parse("1037");
            var client = new GoogleAdsClient();

            //var KeywordTexts = new[] { "Bilgisayar" };
            //var CustomerId = long.Parse("7403111303");
            //var LocationIds = new[] { long.Parse("21069") };
            //var LanguageId = long.Parse("1037");
            //var PageUrl = "https://medium.com/";

            KeywordIdeas codeExample = new KeywordIdeas();
            Console.WriteLine(codeExample.Description);
            Console.WriteLine("1.Authentication");
            Console.WriteLine("2.Keyword Ideas");

            var selected = Console.ReadLine();
            if (selected=="1")
            {
                codeExample.Authentication();
            }
            else
            codeExample.Run(client , options.CustomerId, options.LocationIds.ToArray(), 
                options.LanguageId, options.KeywordTexts.ToArray());
        }
       
    }
}
