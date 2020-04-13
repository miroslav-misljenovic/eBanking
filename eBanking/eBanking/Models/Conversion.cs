namespace eBanking.Models
{
    public class Conversion
    {
        public int CurrencyIdFrom { get; set; }
        public int CurrencyIdTo { get; set; }
        public decimal Amount { get; set; }
    }
}
