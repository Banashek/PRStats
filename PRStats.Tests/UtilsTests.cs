using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

using PRStats;
using System.Collections.Generic;
using System.Linq;

namespace PRStats.Tests
{
    public class UtilsTests
    {
        private static readonly string _token = "token";
        
        [Fact]
        public void getGHCClient_ShouldSetGithubMediaTypeHeader()
        {
            var expectedMediaHeader = new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json");
            var SUT = Utils.getGHClient(_token);
            Assert.Contains(
                expectedMediaHeader,
                SUT.DefaultRequestHeaders.Accept
            );
        }
        
        [Fact]
        public void getGHCClient_ShouldOnlyHaveOneMediaTypeHeader()
        {
            var SUT = Utils.getGHClient(_token);
            Assert.Equal(1, SUT.DefaultRequestHeaders.Accept.Count);
        }

        [Fact]
        public void getGHCClient_ShouldSetUserAgentHeader()
        {
            var expectedHeaderKey = "User-Agent";
            var expectedHeaderValue = "PRStats-Client";

            var SUT = Utils.getGHClient(_token);

            IEnumerable<string> results;
            if (SUT.DefaultRequestHeaders.TryGetValues(expectedHeaderKey, out results))
            {
                Assert.Equal(expectedHeaderValue, results.FirstOrDefault());
            }
            else
            {
                TestUtils.Fail("User-Agent header not found");
            }
        }

        [Fact]
        public void getGHCClient_ShouldSetAuthTokenHeader()
        {
            var expectedHeaderKey = "Authorization";
            var expectedHeaderValue = "Bearer " + _token;

            var SUT = Utils.getGHClient(_token);

            IEnumerable<string> results;
            if (SUT.DefaultRequestHeaders.TryGetValues(expectedHeaderKey, out results))
            {
                Assert.Equal(expectedHeaderValue, results.FirstOrDefault());
            }
            else
            {
                TestUtils.Fail("Authorization header not found");
            }
        }
    }
}
