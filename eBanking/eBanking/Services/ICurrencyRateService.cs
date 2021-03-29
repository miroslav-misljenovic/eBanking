using eBanking.BusinessModels;
using eBanking.Models;
using System.Collections.Generic;

namespace eBanking.Services
{
    public interface ICurrencyRateService
    {
        CurrencyRates GetCurrencyRate();
        void RefreshRates();
        List<Currency> GetCurrencyList();
    }
}
