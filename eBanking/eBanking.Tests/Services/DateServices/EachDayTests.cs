using eBanking.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace eBanking.Tests.Services.DateServices
{
    public class EachDayTests
    {
        public static IEnumerable<DateTime> TestDays()
        {
            yield return new DateTime(2021, 03, 01);
            yield return new DateTime(2021, 03, 02);
            yield return new DateTime(2021, 03, 03);
            yield return new DateTime(2021, 03, 04);
        }
        [Fact]
        public void TestValidDateRange()
        {
            DateService ds = new DateService();
            var ret = ds.EachDay(new DateTime(2021, 03, 01), new DateTime(2021, 03, 04));
            Assert.Equal(TestDays(), ret);
        }

        [Fact]
        public void TestInvalidDateRange()
        {
            DateService ds = new DateService();
            var ret = ds.EachDay(new DateTime(2021, 03, 01), new DateTime(2021, 03, 05));
            Assert.NotEqual(TestDays(), ret);
        }
    }
}
