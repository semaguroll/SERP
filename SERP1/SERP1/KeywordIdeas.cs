using Google.Ads.Gax.Examples;
using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V10.Common;
using Google.Ads.GoogleAds.V10.Errors;
using Google.Ads.GoogleAds.V10.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Google.Ads.GoogleAds.V10.Enums.KeywordPlanNetworkEnum.Types;

namespace SERP1
{
    public class KeywordIdeas : ExampleBase
    {
        private const string GOOGLE_ADS_API_SCOPE = "https://www.googleapis.com/auth/adwords";


        public void Authentication()
        {
            Console.WriteLine("This code example creates an OAuth2 refresh token for the " +
                "Google Ads API .NET Client library. This example works with both web and " +
                "desktop app OAuth client ID types. To use this application\n" +
                  "1) Follow the instructions on " +
                  "https://developers.google.com/google-ads/api/docs/oauth/cloud-project " +
                  "to generate a new client ID and secret.\n" +
                  "2) Run this application.\n" +
                  "3) Enter the client ID and client secret when prompted and follow the instructions.\n" +
                  "4) Once the output is generated, copy its contents into your App.config " +
                  "file. See https://developers.google.com/google-ads/api/docs/client-libs/dotnet/configuration " +
                  "for other configuration options.\n\n");

            Console.WriteLine("IMPORTANT: For web app clients types, you must add " +
                "'http://127.0.0.1/authorize' to the 'Authorized redirect URIs' list in your " +
                "Google Cloud Console project before running this example to avoid getting a " +
                "redirect_uri_mismatch error. Desktop app client types do not require the " +
                "local redirect to be explicitly configured in the console.\n\n");

            // Accept the client ID from user.
            Console.Write("Enter the client ID: ");
            string clientId = Console.ReadLine();

            // Accept the client ID from user.
            Console.Write("Enter the client secret: ");
            string clientSecret = Console.ReadLine();

            // Load the JSON secrets.
            ClientSecrets secrets = new ClientSecrets()
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };

            try
            {
                // Authorize the user using desktop flow. GoogleWebAuthorizationBroker creates a
                // web server that listens to a random port at 127.0.0.1 and the /authorize url
                // as loopback url. See https://github.com/googleapis/google-api-dotnet-client/blob/main/Src/Support/Google.Apis.Auth/OAuth2/LocalServerCodeReceiver.cs
                // for details.
                Task<UserCredential> task = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets,
                    new string[] { GOOGLE_ADS_API_SCOPE },
                    string.Empty,
                    CancellationToken.None,
                    new NullDataStore()
                );
                UserCredential credential = task.Result;

                Console.WriteLine("\nCopy the following content into your App.config file.\n\n" +
                    $"<add key = 'OAuth2Mode' value = 'APPLICATION' />\n" +
                    $"<add key = 'OAuth2ClientId' value = '{clientId}' />\n" +
                    $"<add key = 'OAuth2ClientSecret' value = '{clientSecret}' />\n" +
                    $"<add key = 'OAuth2RefreshToken' value = " +
                    $"'{credential.Token.RefreshToken}' />\n");

                Console.WriteLine("/n" +
                    "<!-- Required for manager accounts only: Specify the login customer -->\n" +
                    "<!-- ID used to authenticate API calls. This will be the customer ID -->\n" +
                    "<!-- of the authenticated manager account. It should be set without -->\n" +
                    "<!-- dashes, for example: 1234567890 instead of 123-456-7890. You can -->\n" +
                    "<!-- also specify this later in code if your application uses -->\n" +
                    "<!-- multiple manager account OAuth pairs. -->\n" +
                    "<add key = 'LoginCustomerId' value = INSERT_LOGIN_CUSTOMER_ID_HERE />/n/n");


                Console.WriteLine("See https://developers.google.com/google-ads/api/docs/client-libs/dotnet/configuration " +
                    "for alternate configuration options.");
                Console.WriteLine("Press <Enter> to continue...");
                Console.ReadLine();
            }
            catch (AggregateException)
            {
                Console.WriteLine("An error occured while authorizing the user.");
            }
        }

        public override string Description =>
            "This code example generates keyword ideas from a list of seed keywords or a seed " +
            "page URL";

        public void Run(GoogleAdsClient client, long customerId, long[] locationIds,
            long languageId, string[] keywordTexts)
        {
            KeywordPlanIdeaServiceClient keywordPlanIdeaService =
                client.GetService(Services.V10.KeywordPlanIdeaService);

            // Make sure that keywords and/or page URL were specified. The request must have
            // exactly one of urlSeed, keywordSeed, or keywordAndUrlSeed set.
            //&& string.IsNullOrEmpty(pageUrl)
            if (keywordTexts.Length == 0 )
            {
                throw new ArgumentException("At least one of keywords or page URL is required, " +
                    "but neither was specified.");
            }

            // Specify the optional arguments of the request as a keywordSeed, UrlSeed,
            // or KeywordAndUrlSeed.
            GenerateKeywordIdeasRequest request = new GenerateKeywordIdeasRequest()
            {
                CustomerId = customerId.ToString(),
            };

            if (keywordTexts.Length == 0)
            {
                // Only page URL was specified, so use a UrlSeed.
                request.UrlSeed = new UrlSeed()
                {
                    //Url = pageUrl
                };
            }
            //else if (string.IsNullOrEmpty(pageUrl))
            //{
            //    // Only keywords were specified, so use a KeywordSeed.
            //    request.KeywordSeed = new KeywordSeed();
            //    request.KeywordSeed.Keywords.AddRange(keywordTexts);
            //}
            else
            {
                // Both page URL and keywords were specified, so use a KeywordAndUrlSeed.
                request.KeywordSeed = new KeywordSeed();
                //request.KeywordAndUrlSeed.Url = pageUrl;
                request.KeywordSeed.Keywords.AddRange(keywordTexts);
            }

            // Create a list of geo target constants based on the resource name of specified
            // location IDs.
            foreach (long locationId in locationIds)
            {
                request.GeoTargetConstants.Add(ResourceNames.GeoTargetConstant(locationId));
            }

            request.Language = ResourceNames.LanguageConstant(languageId);
            // Set the network. To restrict to only Google Search, change the parameter below to
            // KeywordPlanNetwork.GoogleSearch.
            request.KeywordPlanNetwork = KeywordPlanNetwork.GoogleSearchAndPartners;

            try
            {
                // Generate keyword ideas based on the specified parameters.
                var response =
                    keywordPlanIdeaService.GenerateKeywordIdeas(request);

                // Iterate over the results and print its detail.
                foreach (GenerateKeywordIdeaResult result in response)
                {
                    KeywordPlanHistoricalMetrics metrics = result.KeywordIdeaMetrics;
                    Console.WriteLine($"Keyword idea text '{result.Text}' has " +
                        $"{metrics.AvgMonthlySearches} average monthly searches and competition " +
                        $"is {metrics.Competition}.");
                }
            }
            catch (GoogleAdsException e)
            {
                Console.WriteLine("Failure:");
                Console.WriteLine($"Message: {e.Message}");
                Console.WriteLine($"Failure: {e.Failure}");
                Console.WriteLine($"Request ID: {e.RequestId}");
                throw;
            }
        }

    }
}
