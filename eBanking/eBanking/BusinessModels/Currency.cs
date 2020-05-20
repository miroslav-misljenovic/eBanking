using System.Collections.Generic;

namespace eBanking.BusinessModels
{
    public class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BankAccount> Accounts { get; set; }
        public double Rate { get; set; }
        public List<CurrencyRateHistory> History { get; set; }
    }
}
