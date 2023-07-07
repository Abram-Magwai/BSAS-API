namespace bsas.core.models
{
    public class StatementDetails : IComparable<StatementDetails>
    {
        public int StatementNumber { get; set; }
        public List<Transaction> Transactions { get; set; } = new();

        public int CompareTo(StatementDetails? obj)
        {
            var statementDetails = obj;
            if (StatementNumber == statementDetails!.StatementNumber) return 0;
            return StatementNumber < statementDetails.StatementNumber ? -1 : 1;
        }
    }
}