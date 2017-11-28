using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PRStats
{
    public class PullRequest
    {
        [JsonProperty]
        public int Number { get; set; }
        
        [JsonProperty("merged_at")]
        public DateTime? MergedAt { get; set; }
        
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonProperty("closed_at")]
        public DateTime? ClosedAt { get; set; }

        [JsonProperty("commits_url")]
        public string CommitsUrl { get; set; }

        public List<Commit> Commits { get; set; }

        public string State { get; set; }

        public TimeSpan CreationToMergeTime()
        {
            if (!MergedAt.HasValue)
            {
                return DateTime.Now.Subtract(CreatedAt);
            }
            else
            {
                return MergedAt.Value.Subtract(CreatedAt);
            }
        }

        public TimeSpan FirstCommitToMergeTime()
        {
            if (!MergedAt.HasValue)
            {
                return DateTime.Now.Subtract(Commits.First().Details.Committer.CommitDate);
            }
            else
            {
                return MergedAt.Value.Subtract(Commits.First().Details.Committer.CommitDate);
            }
        }
    }
}