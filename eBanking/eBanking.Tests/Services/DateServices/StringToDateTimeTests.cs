using Xunit;
using eBanking.Services;
using System;

namespace eBanking.Tests.Services.DateServices
{
    public class StringToDateTimeTests
    {
        [Fact]
        public void TestValidString()
        {
            DateService ds = new DateService();
            DateTime dt = ds.StringToDateTime("1993-06-02");
            Assert.Equal(new DateTime(1993, 06, 02), dt);
        }

        [Fact]
        public void Test29February2021_ThrowsRangeExeception()
        {
            DateService ds = new DateService();
            Assert.Throws<ArgumentOutOfRangeException>(() => ds.StringToDateTime("2021-02-29"));
        }

        [Fact]
        public void TestInvalidString_ThrowsRangeExeception()
        {
            DateService ds = new DateService();
            Assert.Throws<ArgumentOutOfRangeException>(() => ds.StringToDateTime("1993-13-02"));
        }
        [Fact]
        public void TestEmptyString_ThrowsFormatExeception()
        {
            DateService ds = new DateService();
            Assert.Throws<FormatException>(() => ds.StringToDateTime(""));
        }
    }
}
