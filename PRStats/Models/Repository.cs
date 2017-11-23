using System.Collections.Generic;
using Newtonsoft.Json;

namespace PRStats
{
    public class Repository
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty("pulls_url")]
        public string PullsUrl { get; set; }

        // Removes "{/number} from the end of the pulls_url retrieved from github and adds appropriate query parameters
        public string pullUrlForPage(int page)
        {
            var baseUrl = PullsUrl.Substring(0, PullsUrl.LastIndexOf('{'));
            var urlParams = "?state=all&per_page=100&page=";
            return baseUrl + urlParams + page.ToString();
        }
    }

    public class RepositoryComparer : IEqualityComparer<Repository>
    {
        public bool Equals(Repository r1, Repository r2)
        {
            if (r1 == null && r2 == null)
            {
                return true;
            }
            else if (r1 == null | r2 == null)
            {
                return false;
            }
            else if ((r1.Id == r2.Id) && (r1.Name == r2.Name) && (r1.PullsUrl == r2.PullsUrl))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(Repository r)
        {
            int hCode = r.Id ^ r.Name.Length;
            return hCode.GetHashCode();
        }
    }
}