using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRStats;

namespace PRStats
{
    public static class PullRequestInfo
    {
        public static void LogNumberOfPRsMergedWeekOverWeekByCreatedAt(IEnumerable<PullRequest> prs, DateTime fromDate)
        {
            var prsByWeek = PRsWeekOverWeekByCreatedAt(prs, fromDate);
            
            Console.WriteLine("Pull Requests merged week-over-week (by created_at date)");
            Console.WriteLine("--------------------------------------------------------");
            foreach (var w in prsByWeek)
            {
                Console.WriteLine("The week of " + w.Key.ToShortDateString() + " had " + w.Value.Count() + " merged Pull Requests");
                foreach (var pr in w.Value)
                {
                    string sfx = pr.MergedAt.HasValue ? " and was merged at " + pr.MergedAt.Value.ToShortDateString() : "and is still open";
                    Console.WriteLine("\t" + pr.Number + " was created at " + pr.CreatedAt.ToShortDateString() + sfx);
                }
            }
        }

        public static void LogCreationToMergeTimeAverageWeekOverWeekByCreatedAt(IEnumerable<PullRequest> prs, DateTime dt)
        {
            var prsByWeek = PRsWeekOverWeekByCreatedAt(prs, dt);

            Console.WriteLine("Average time between creation and merge of a Pull Request week-over-week (open PRs counted) from " + dt.ToShortDateString() + " until now");
            Console.WriteLine("--------------------------------------------------------");
            foreach (var w in prsByWeek)
            {
                System.Console.WriteLine(w.Key.ToShortDateString() + " has " + w.Value.Count.ToString() + " values.");
                string avg;
                if (w.Value.Count > 0)
                {
                    foreach (var pr in w.Value)
                    {
                        string mDate = pr.MergedAt.HasValue ? pr.MergedAt.Value.ToShortDateString() : "never";
                        System.Console.WriteLine("\t" + pr.Number + " was created at " + pr.CreatedAt.ToShortDateString() + " and was merged at " + mDate + " and has a state of " + pr.State + " for a timespan of " + timeSpanToDaysAndHoursString(pr.CreationToMergeTime()));
                    }

                    avg = w.Value.Aggregate(new TimeSpan(), (x, y) => x.Add(y.CreationToMergeTime()), (x) => x).TotalHours.ToString("N1");
                }
                else
                {
                    avg = "0";
                }
                Console.WriteLine(w.Key.ToShortDateString() + " has an average create-to-merge time of " + avg + " hours");
            }
        }

        public static async Task LogFirstCommitToMergeTimeAverageWeekOverWeekByCreatedAt(PullRequester pr, IEnumerable<PullRequest> prs, DateTime dt)
        {
            var prsByWeek = PRsWeekOverWeekByCreatedAt(prs, dt);
            Console.WriteLine("Average time between first commit and merge of a Pull Request week-over-week (open PRs counted) from " + dt.ToShortDateString() + " until now");
            Console.WriteLine("--------------------------------------------------------");
            foreach (var w in prsByWeek)
            {
                System.Console.WriteLine(w.Key.ToShortDateString() + " has " + w.Value.Count.ToString() + " values.");
                string avg;
                if (w.Value.Count > 0)
                {
                    foreach (var p in w.Value)
                    {
                        p.Commits = await pr.GetFirstCommitForPR(p);
                        string mDate = p.MergedAt.HasValue ? p.MergedAt.Value.ToShortDateString() : "never";
                        System.Console.WriteLine("\t" + p.Number + "'s first commit " + p.Commits.First().Sha + " was created at " + p.Commits.First().Details.Committer.CommitDate.ToShortDateString() + " and was merged at " + mDate +  " for a timespan of " + timeSpanToDaysAndHoursString(p.FirstCommitToMergeTime()));
                    }

                    avg = w.Value.Aggregate(new TimeSpan(), (x, y) => x.Add(y.CreationToMergeTime()), (x) => x).TotalHours.ToString("N1");
                }
                else
                {
                    avg = "0";
                }
                Console.WriteLine(w.Key.ToShortDateString() + " has an average create-to-merge time of " + avg + " hours");
            }
        }






        private static Dictionary<DateTime, List<PullRequest>> PRsWeekOverWeekByCreatedAt(IEnumerable<PullRequest> prs, DateTime fromDate)
        {
            return PRsWeekOverWeekByCreatedAt(prs, fromDate, DateTime.Now);
        }

        private static Dictionary<DateTime, List<PullRequest>> PRsWeekOverWeekByCreatedAt(IEnumerable<PullRequest> prs, DateTime fromDate, DateTime toDate)
        {
            var prsByDate = new Dictionary<DateTime, List<PullRequest>>();

            var currentDate = firstDayOfWeekForDate(fromDate);
            while (currentDate < DateTime.Now)
            {
                prsByDate.Add(currentDate, new List<PullRequest>());
                currentDate = firstDayOfWeekForDate(currentDate.AddDays(7));
            }

            prs = prs
                .Where(pr => pr.CreatedAt > fromDate && pr.CreatedAt < toDate && (pr.MergedAt.HasValue || !pr.ClosedAt.HasValue));

            foreach (var pr in prs)
            {
                prsByDate[firstDayOfWeekForDate(pr.CreatedAt)].Add(pr);
            }

            return prsByDate;
        }

        private static DateTime firstDayOfWeekForDate(DateTime dt)
        {
            int daysDiff = dt.DayOfWeek - DayOfWeek.Monday;
            daysDiff = daysDiff < 0 ? daysDiff + 7 : daysDiff;
            return dt.AddDays(-1 * daysDiff).Date;
        }

        private static string timeSpanToDaysAndHoursString(TimeSpan ts)
        {
            var sb = new StringBuilder();
            if (ts.Days > 0)
            {
                sb.Append(ts.Days + " days and ");
            }
            if (ts.Hours > 0)
            {
                sb.Append(ts.Hours + " hours");
            }
            if (ts.Days < 1 && ts.Hours < 1)
            {
                sb.Append("less than one hour.");
            }
            return sb.ToString();
        }
    }
}