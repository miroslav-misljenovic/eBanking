namespace eBanking.BusinessModels
{
    public class Transaction
    {
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public double Amount { get; set; }
        public TransactionStatus Status { get; set; } 
    }
}
