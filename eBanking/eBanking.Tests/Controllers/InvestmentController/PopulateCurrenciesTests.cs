using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.Collections.Generic;
using eBanking.BusinessModels;
using eBanking.Data;
using eBanking.Controllers;

namespace eBanking.Tests.Controllers.InvestmentControllerA
{
    public class PopulateCurrenciesTests
    {
        List<Currency> entities;

        [Fact]
        public void EachDayTest()
        {
            entities = new List<Currency>();
            entities.Add(new Currency
            {
                Id = 1,
                Name = "MIKI",
                Rate = -3.5
            });
            var myDbMoq = new Mock<ApplicationDbContext>();
            myDbMoq.Setup(p => p.Currencies).Returns(
                DbContextMock.GetQueryableMockDbSet<Currency>(entities));
            myDbMoq.Setup(p => p.SaveChanges()).Returns(1);
            InvestmentController iCon = new InvestmentController(myDbMoq.Object);
            var ret = iCon.GetCurrencyList();
            Assert.NotNull(ret);
        }
    }
}
