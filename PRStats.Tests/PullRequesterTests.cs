using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

using PRStats;
using System.Collections.Generic;
using System.Linq;

namespace PRStats.Tests
{
    /*
     * The tests for this class are poor and rely on actual API requests. With more time to work on this project,
     * I would fix this depending on the application architecture direction. Ideally, everything would be made a
     * lot more functional and we could do simple unit tests on the separated behavior. In my experience however,
     * the functional approach can be a bit alien for teams used to a more OOP approach. If that were the case,
     * I would pull out certain dependencies and mock them (API requests) using a mocking framework like Moq
     * and Assert that the methods were called with appropriate parameters. Just squint and they look like nice
     * Functional/Integration tests.
     * Also note that the `GH_TOKEN` environment variable must be set, otherwise a 401 response will be received
     * and the tests will fail. 
     */
    public class PullRequesterTests
    {
        private static readonly string _token = Environment.GetEnvironmentVariable("GH_TOKEN");
        private static readonly string _orgName = "lodash";
        
        [Fact]
        public async void PullRequester_GetPRsByPageForRepository_ShouldGetResults()
        {
            var expectedPrsCount = 100;
            var repo = new Repository();
            repo.PullsUrl = "https://api.github.com/repos/lodash/lodash/pulls{/number}";

            var SUT = new PullRequester(_token, _orgName, Utils.getGHClient);
            var results = await SUT.GetPRsByPageForRepository(repo, 1);

            Assert.Equal(expectedPrsCount, results.Count());
        }

        [Fact]
        public async void PullRequester_GetAllPRsForRepo_ShouldGetMultiplePagesOfResultsForRepo()
        {
            var expectedMinimumPrsCount = 100;
            var repo = new Repository();
            repo.PullsUrl = "https://api.github.com/repos/lodash/lodash/pulls{/number}";
            
            var SUT = new PullRequester(_token, _orgName, Utils.getGHClient);
            var results = await SUT.GetAllPRsForRepo(repo);

            Assert.True(results.Value.Count() > expectedMinimumPrsCount);
        }

        [Fact]
        public async void PullRequester_getRepoUrls_ShouldGetRepoNamesForOrg()
        {
            var expectedRepo = getLodashTestRepo();
            
            var SUT = new PullRequester(_token, _orgName, Utils.getGHClient);
            var results = await SUT.getRepoUrls();

            Assert.Contains(expectedRepo, results.ToList(), new RepositoryComparer());
        }

        [Fact]
        public async void PullRequester_GetAllPRsForOrg_ShouldGetAllPRsForOrg()
        {
            var expectedRepo = getLodashTestRepo();
            var SUT = new PullRequester(_token, _orgName, Utils.getGHClient);
            var results = await SUT.GetAllPRsForOrg();

            var comparer = new RepositoryComparer();
            Assert.True(results.Repositories.Keys.Any(r => comparer.Equals(expectedRepo, r)));
        }

        private Repository getLodashTestRepo()
        {
            var repo = new Repository();
            repo.Id = 3955647;
            repo.Name = "lodash";
            repo.PullsUrl = "https://api.github.com/repos/lodash/lodash/pulls{/number}";
            return repo;
        }
    }
}
