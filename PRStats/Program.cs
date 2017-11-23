using System;
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
            Console.WriteLine("Results for the " + orgname + " organization");
            Console.WriteLine("--------------------------------------------");
            foreach (var r in prs.Repositories)
            {
                Console.WriteLine(r.Key.Name + " has " + r.Value.ToList().Count.ToString() + " total pull requests.");
            }
            Console.WriteLine("\n\n");

            // How many pull requests were merged week over week across the organization
            Console.WriteLine("Weekly pull request overview for all repos under the " + orgname + " organization\n");
            Console.WriteLine("Week of:\tNo. of PRs");
            Console.WriteLine("--------\t----------");
            prs.Repositories
                .SelectMany(r => r.Value) // Extract just the pull requests
                .Where(pr => pr.MergedAt.HasValue)
                .OrderBy(pr => pr.MergedAt)
                .GroupBy(pr => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(pr.MergedAt.Value, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                .ToList()
                // .ForEach(g => Console.WriteLine("There are " + g.Count() + " merged pull requests for the week beginning " + firstDayOfWeekForDate(g.First().MergedAt.Value.Date).ToShortDateString()));
                .ForEach(g => Console.WriteLine(firstDayOfWeekForDate(g.First().MergedAt.Value.Date).ToShortDateString() + "  \t" + g.Count()));
        }

        private static DateTime firstDayOfWeekForDate(DateTime dt)
        {
            int daysDiff = dt.DayOfWeek - DayOfWeek.Monday;
            daysDiff = daysDiff < 0 ? daysDiff + 7 : daysDiff;
            return dt.AddDays(-1 * daysDiff).Date;
        }
    }
}