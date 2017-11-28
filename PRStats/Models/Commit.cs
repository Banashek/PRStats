using System;
using Newtonsoft.Json;

namespace PRStats
{
    public class Commit
    {
        [JsonProperty("commit")]
        public CommitDetails Details { get; set; }

        [JsonProperty("sha")]
        public string Sha { get; set; }
    }

    public class CommitDetails
    {
        [JsonProperty("committer")]
        public CommitCommiter Committer { get; set; }
    }

    public class CommitCommiter
    {
        [JsonProperty("Date")]
        public DateTime CommitDate { get; set; }
    }
}