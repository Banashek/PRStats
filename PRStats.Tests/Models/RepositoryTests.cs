using Xunit;

namespace PRStats.Tests
{
    public class RepositoryTests
    {
        
        [Fact]
        public void repository_pullUrlForPage_ShouldProperlyFormatUrl()
        {
            var originalUrl = "http://api.github.com/repos/octocat/Hello-World/pulls{/number}";
            var expectedUrlForPage1 = "http://api.github.com/repos/octocat/Hello-World/pulls?state=all&per_page=100&page=1";

            var SUT = new Repository();
            SUT.PullsUrl = originalUrl;

            Assert.Equal(expectedUrlForPage1, SUT.pullUrlForPage(1));
        }
    }
}
