using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PRStats
{
    public class PullRequester
    {
        private static readonly string BASE_API_URL = "https://api.github.com";
        private string _token { get; set; }
        private string _orgName { get; set; }
        private Func<string, HttpClient> _getHttpClient { get; set; }

        public PullRequester(string token, string orgName, Func<string, HttpClient> httpClientFunc)
        {
            _orgName = orgName;
            _token = token;
            _getHttpClient = httpClientFunc;
        }

        public async Task<PRResults> GetAllPRsForOrg()
        {
            var allPRs = new PRResults(_orgName);
            
            var repos = await getRepoUrls();
            var tasks = repos.Select(r => GetAllPRsForRepo(r));
            var results = await Task.WhenAll(tasks);

            if (results.Any())
            {
                foreach (var r in results)
                {
                    allPRs.Repositories.Add(r.Key, r.Value);
                }
            // TODO: Add proper error logging here
            } else { /* System.Console.WriteLine("Results is empty..."); */ }

            return allPRs;
        }

        public async Task<IEnumerable<Repository>> getRepoUrls()
        {
            var client = _getHttpClient(_token);
            var res = await client.GetStringAsync(BASE_API_URL + "/orgs/" + _orgName + "/repos");
            var repos = JsonConvert.DeserializeObject<IEnumerable<Repository>>(res);
            return repos;
        }

        public async Task<KeyValuePair<Repository, IEnumerable<PullRequest>>> GetAllPRsForRepo(Repository repo)
        {
            var prs = Enumerable.Empty<PullRequest>();

            // Make an initial manual request to get the total number of pages, then handle the rest concurrently
            var client = _getHttpClient(_token);
            var firstPageUrl = repo.pullUrlForPage(1);
            var res = await client.GetAsync(firstPageUrl);

            // Add the data from the first page to the return object
            var firstPageContent = await res.Content.ReadAsStringAsync();
            var firstPagePrs = JsonConvert.DeserializeObject<IEnumerable<PullRequest>>(firstPageContent); 
            prs = prs.Concat(firstPagePrs);

            // The `Link` header is only present if there are more pages (makes sense, but is undocumented in the API)
            // Grab and parse this header if available, then make async calls to gather the rest of the pages
            IEnumerable<string> linkHeaders;
            if(res.Headers.TryGetValues("Link", out linkHeaders))
            {
                var linkHeader = linkHeaders.FirstOrDefault();
                if (!String.IsNullOrEmpty(linkHeader))
                {
                    var regex = new Regex(@"page=(\d*)>; rel=.?last");
                    var match = regex.Match(linkHeader);
                    int lastPageNum;
                    if (int.TryParse(match.Groups[1].Value, out lastPageNum))
                    {
                        // Async calls for the rest of the pages
                        var tasks = Enumerable.Range(2, lastPageNum-1).Select(i => GetPRsByPageForRepository(repo, i));
                        var results = await Task.WhenAll(tasks);
                        foreach (var r in results)
                        {
                            prs = prs.Concat(r);
                        }
            // TODO: ADD PROPER LOGGING/Handling IN THESE FAILURE CASES
                    } else { /* Console.WriteLine("Unable to parse number of pages from: " + linkHeader); */ }
                } else { /* Console.WriteLine("Link header was found, but is empty"); */ }
            } else { /* Console.WriteLine("Could not find link header for " + repo.Name); */ }

            return new KeyValuePair<Repository, IEnumerable<PullRequest>>(repo, prs);
        }

        public async Task<IEnumerable<PullRequest>> GetPRsByPageForRepository(Repository repo, int page)
        {
            var client = _getHttpClient(_token);
            var url = repo.pullUrlForPage(page);
            var res = await client.GetStringAsync(url);
            var prs = JsonConvert.DeserializeObject<IEnumerable<PullRequest>>(res);
            return prs;
        }
    }
}