namespace bsas.core.models
{
    public class TransactionSummary
    {
        public string? Description { get; set; }
        public double Total {get;set;} = 0;
        public int Visits {get;set;} = 0;
        public TransactionSummary()
        {
            Visits = 1;
        }
    }
}