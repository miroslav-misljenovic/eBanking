using System;
using Xunit;
using eBanking.Services;

namespace eBanking.Tests.Services
{
    public class DateTimeToStringTests
    {
        [Fact]
        public void Test1700101()
        {
            DateService hc = new DateService();
            string s = hc.DateTimeToString(new DateTime(1970, 1, 1));
            Assert.Equal("1970-01-01", s);
        }
    }
}
