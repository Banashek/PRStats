using Xunit;

namespace PRStats.Tests
{
    internal static class TestUtils
    {
        internal static void Fail(string message)
        {
            Assert.True(false, message);
        }
    }
}