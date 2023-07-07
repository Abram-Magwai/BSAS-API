namespace bsas.core.models
{
    public class CommonTransaction
    {
        public string Description { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public double Minimum { get; set; } = double.MaxValue;
        public double Maximum { get; set; } = double.MinValue;
    }
}