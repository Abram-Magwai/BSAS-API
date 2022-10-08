namespace bsas.core.models
{
    public class Transaction
    {
        public string? Date { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? TransactionType { get; set; } = string.Empty;
        public double Amount { get; set; } = 0.0;
        public double Balance { get; set; } = 0.0;
    }
}