using System;
using Xunit;
using eBanking.Services;

namespace eBanking.Tests.Services.DateServices
{
    public class DateTimeToStringTests
    {
        [Fact]
        public void TestValidDateTime()
        {
            DateService ds = new DateService();
            string s = ds.DateTimeToString(new DateTime(1970, 1, 1));
            Assert.Equal("1970-01-01", s);
        }
        [Fact]
        public void Test29February2021_ThrowsRangeExeception()
        {
            DateService ds = new DateService();
            Assert.Throws<ArgumentOutOfRangeException>(() => ds.DateTimeToString(new DateTime(2021,02,29)));
        }

        [Fact]
        public void TestInalidDateTime()
        {
            DateService ds = new DateService();
            Assert.Throws<ArgumentOutOfRangeException>(() => ds.DateTimeToString(new DateTime(1970, 13, 1)));
        }
    }
}
