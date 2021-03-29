using eBanking.Models;
using System.Collections.Generic;

namespace eBanking.Services
{
    public interface IExchangeRatesService
    {
        List<ExchangeRateOnDate> PrepareRates(ExchangeRateRequest req);
    }
}
