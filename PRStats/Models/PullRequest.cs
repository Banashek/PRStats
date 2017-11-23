using System;
using Newtonsoft.Json;

namespace PRStats
{
    public class PullRequest
    {
        [JsonProperty]
        public int Number { get; set; }
        
        [JsonProperty("merged_at")]
        public DateTime? MergedAt { get; set; }
    }
}