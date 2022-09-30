namespace bsas.core.models
{
    public class Transaction
    {
        public string? Date { get; set; }
        public string? Description { get; set; }
        public string? TransactionType { get; set; }
        public double Amount { get; set; }
        public double Balance { get; set; }
    }
}