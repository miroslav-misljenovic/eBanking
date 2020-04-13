namespace eBanking.Models
{
    public class Transaction
    {
        public int SenderCurrencyId { get; set; }
        public double Amount { get; set; }
        public int RecipientAccount { get; set; }
    }
}