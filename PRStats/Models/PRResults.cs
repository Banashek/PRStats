using System.Collections.Generic;

namespace PRStats
{
    public class PRResults
    {
        public string OrgName { get; set; }
        public Dictionary<Repository, IEnumerable<PullRequest>> Repositories { get; set; }

        public PRResults(string orgName)
        {
            OrgName = orgName;
            Repositories = new Dictionary<Repository, IEnumerable<PullRequest>>();
        }
    }
}