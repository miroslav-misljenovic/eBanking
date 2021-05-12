using eBanking.BusinessModels;
using eBanking.Data;
using eBanking.Models;
using eBanking.Services;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace eBanking.Tests.Services.ExchangeRatesServices
{
    public class PrepareRatesTests
    {
        [Fact]
        public void Test28032021()
        {
            Currency currency = new Currency
            {
                Id = 1,
                Name = "CAD"
            };
            List<Currency> currencies = new List<Currency> { currency };
            List<CurrencyRateHistory> entities = new List<CurrencyRateHistory>();
            entities.Add(new CurrencyRateHistory
            {
                Date = new DateTime(2021, 03, 28),
                CurrencyId = 1,
                Id = 99,
                Rate = 88.00,
                Currency = currency
            });          

            var myDbMoq = new Mock<ApplicationDbContext>();
            myDbMoq.Setup(p => p.CurrencyRateHistory).Returns(
                DbContextMock.GetQueryableMockDbSet(entities));
            myDbMoq.Setup(p => p.Currencies).Returns(
                DbContextMock.GetQueryableMockDbSet(currencies));
            myDbMoq.Setup(p => p.SaveChanges()).Returns(2);
            ExchangeRatesService ers = new ExchangeRatesService(myDbMoq.Object);

            var ret = ers.PrepareRates(
                new ExchangeRateRequest{Date = new DateTime(2021, 03, 28)});
                        
            Assert.True(ret.Exists(x => x.Rate == 88.00 & x.Name.Equals("CAD")));
        }
    }
}