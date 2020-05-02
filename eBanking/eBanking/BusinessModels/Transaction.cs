namespace eBanking.BusinessModels
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountFromId { get; set; }
        public BankAccount AccountFrom { get; set; }
        public int AccountToId { get; set; }
        public BankAccount AccountTo { get; set; }
        public double Amount { get; set; }
        public TransactionStatus Status { get; set; } 
    }
}
