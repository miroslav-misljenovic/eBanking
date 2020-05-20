using System;

namespace eBanking.BusinessModels
{
    public class CurrencyRateHistory
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public DateTime Date { get; set; }
        public double Rate { get; set; }

    }
}
