using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PRStats
{
    class Program
    {
        static void Main(string[] args)
        {
            string githubAccessToken = Environment.GetEnvironmentVariable("GH_TOKEN");
            if(githubAccessToken == null)
            {
                Console.WriteLine("The environment variable GH_TOKEN was not found." +
                    "Please set this to your github access token.");
                Environment.Exit(-1);
            }

            SampleUsage(githubAccessToken).Wait();
        }

        public static async Task SampleUsage(string token)
        {
            // Sample Usage of Library
            var orgname = "lodash";

            // Retrieve all pull requests into memory
            var pullRequester = new PullRequester(token, orgname, Utils.getGHClient);
            var prs = await pullRequester.GetAllPRsForOrg();

            // Print how many pull requests each repository has
            // Console.WriteLine("Results for the " + orgname + " organization");
            // Console.WriteLine("--------------------------------------------");
            // foreach (var r in prs.Repositories)
            // {
            //     Console.WriteLine(r.Key.Name + " has " + r.Value.ToList().Count.ToString() + " total pull requests.");
            // }
            // Console.WriteLine("\n\n");


            

            // The following is a demo for just the Lodash repository
            var pullRequestsForLodash = prs.Repositories.First().Value;
            var testDate = new DateTime(2017, 10, 1);

            // How many pull requests week over week since 2017-08-01 ?
            PullRequestInfo.LogNumberOfPRsMergedWeekOverWeekByCreatedAt(pullRequestsForLodash, testDate);
            Console.WriteLine("\n\n");

            // What was the average time between the creation and the merge of a pull request? 
            // (Open pull requests that have not been closed are weighted based on the time between their created_at date and the current date)
            PullRequestInfo.LogCreationToMergeTimeAverageWeekOverWeekByCreatedAt(prs.Repositories.First().Value, testDate);
            Console.WriteLine("\n\n");

            // What was the average time between the creation of the first commit and the merge of a pull request? 
            // (Open pull requests that have not been closed are weighted based on the time between their commit's created_at date and the current date)
            PullRequestInfo.LogFirstCommitToMergeTimeAverageWeekOverWeekByCreatedAt(pullRequester, prs.Repositories.First().Value, testDate).Wait();
            Console.WriteLine("\n\n");
        }
    }
}
