using eBanking.Data;
using eBanking.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace eBanking.Services
{
    public class ExchangeRatesService : IExchangeRatesService
    {
        private readonly ApplicationDbContext _dbContext;

        public ExchangeRatesService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<ExchangeRateOnDate> PrepareRates(ExchangeRateRequest req)
        {
            var a = _dbContext.CurrencyRateHistory
                .Include(a => a.Currency)
                .Where(a => a.Date.Year == req.Date.Year &&
                            a.Date.Month == req.Date.Month &&
                            a.Date.Day == req.Date.Day)
                .ToList();
            var result = a.Select(x => new ExchangeRateOnDate { Name = x.Currency.Name, Rate = x.Rate }).ToList();

            return result;
        }
    }
}