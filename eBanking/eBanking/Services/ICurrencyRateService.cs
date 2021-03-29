using eBanking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eBanking.Services
{
    public interface ICurrencyRateService
    {
        CurrencyRates GetCurrencyRate();

        void RefreshRates();

    }
}
