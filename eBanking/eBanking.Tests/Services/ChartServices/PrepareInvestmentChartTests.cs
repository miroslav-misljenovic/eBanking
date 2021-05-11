using eBanking.BusinessModels;
using eBanking.Data;
using eBanking.Models;
using eBanking.Services;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace eBanking.Tests.Services.ChartServices
{
    public class PrepareInvestmentChartTests
    {
        InvestmentChart ic = new InvestmentChart {
            FirstCurrency = 1,
            SecondCurrency = 5,
            StartingDate = new DateTime(2021, 03, 23),
            EndingDate = new DateTime(2021, 03, 26),
            Alpha = 0.5,
            RadioResponse = "ExponentialSmoothing"
        };

        [Fact]
        public void TestPrepareInvestmentChartRendering()
        {
            Currency currency1 = new Currency
            {
                Id = 5,
                Name = "DINAR"
            };
            Currency currency5 = new Currency
            {
                Id = 1,
                Name = "EURO"
            };

            List<Currency> currencies = new List<Currency> {currency1, currency5 };
            List<CurrencyRateHistory> entities = new List<CurrencyRateHistory>();
            entities.Add(new CurrencyRateHistory
            {
                Date = new DateTime(2021, 03, 23),
                CurrencyId = 5,
                Id = 123,
                Rate = 2.22,
                Currency = currency5
            });
            entities.Add(new CurrencyRateHistory
            {
                Date = new DateTime(2021, 03, 23),
                CurrencyId = 1,
                Id = 124,
                Rate = 3.22,
                Currency = currency1
            });
            entities.Add(new CurrencyRateHistory
            {
                Date = new DateTime(2021, 03, 24),
                CurrencyId = 5,
                Id = 125,
                Rate = 4.22,
                Currency = currency5
            }); 
            entities.Add(new CurrencyRateHistory
            {
                Date = new DateTime(2021, 03, 24),
                CurrencyId = 1,
                Id = 126,
                Rate = 5.22,
                Currency = currency1
            });

            var myDbMoq = new Mock<ApplicationDbContext>();
            myDbMoq.Setup(p => p.CurrencyRateHistory).Returns(
                DbContextMock.GetQueryableMockDbSet(entities));
            myDbMoq.Setup(p => p.Currencies).Returns(
                DbContextMock.GetQueryableMockDbSet(currencies));
            myDbMoq.Setup(p => p.SaveChanges()).Returns(4);
            ChartService cs = new ChartService(myDbMoq.Object);
            
            var chart = cs.PrepareInvestmentChart(ic);

            Assert.Contains("\"dataSource\":{\"chart\":{\"xAxisName\":\"Date\",\"pYAxisName\":\"DINAR\",\"sYAxisName\":\"EURO\",\"caption\":\"Investment consulting\",\"theme\":\"fusion\"},\"categories\":[{\"category\":[{\"label\":\"23.03.2021\"},{\"label\":\"24.03.2021\"}]}],\"dataset\":[{\"seriesname\":\"FirstCurrencyValue\",\"renderAs\":\"line\",\"data\":[{\"value\":\"322\"},{\"value\":\"522\"}]},{\"seriesname\":\"SecondCurrencyValue\",\"renderAs\":\"line\",\"parentYAxis\":\"S\",\"data\":[{\"value\":\"222.00000000000003\"},{\"value\":\"422\"}]},{\"seriesname\":\"ThirdCurrencyValue\",\"renderAs\":\"column\",\"data\":[{\"value\":\"0\"},{\"value\":\"522\"}]}"
                , chart);
        }
    }
}
