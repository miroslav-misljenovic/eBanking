using System.Collections.Generic;
using Xunit;
using Moq;
using eBanking.BusinessModels;
using eBanking.Data;
using eBanking.Controllers;

namespace eBanking.Tests.Controllers.InvestmentControllerA
{
    public class PopulateCurrenciesTests
    {
        List<Currency> entities;

        [Fact]
        public void TestGetCurrencyList()
        {
            entities = new List<Currency>();
            entities.Add(new Currency
            {
                Id = 1,
                Name = "MIKI",
                Rate = -3.5
            });
            entities.Add(new Currency
            {
                Id = 2,
                Name = "ZOKI"
            });
            var myDbMoq = new Mock<ApplicationDbContext>();
            myDbMoq.Setup(p => p.Currencies).Returns(
                DbContextMock.GetQueryableMockDbSet<Currency>(entities));
            myDbMoq.Setup(p => p.SaveChanges()).Returns(1);
            InvestmentController iCon = new InvestmentController(myDbMoq.Object);
            
            var ret = iCon.GetCurrencyList();
            
            Assert.NotNull(ret);
            Assert.True(ret.Exists(x => x.Id == 1 && x.Name.Equals("MIKI") && x.Rate == -3.5), "Nepostojeci element");
            Assert.True(ret.Exists(x => x.Id == 2 && x.Name.Equals("ZOKI") && x.Rate == 0.0), "Nepostojeci element");
        }
    }
}
