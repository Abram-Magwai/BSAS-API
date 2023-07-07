using bsas.core.models;

namespace bsas.core.interfaces
{
    public interface IFnbService
    {
        StatementDetails GetStatementDetails(byte[] fileBytes);
        List<TransactionSummary> GetTransactionSummaries(List<Transaction> transactions);
        List<TransactionSummary> Compare(List<TransactionSummary> first, List<TransactionSummary> second);
    }
}