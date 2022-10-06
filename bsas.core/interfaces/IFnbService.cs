using bsas.core.models;

namespace bsas.core.interfaces
{
    public interface IFnbService
    {
        public List<Transaction> GetTransactions(byte[] fileBytes);
        List<TransactionSummary> GetTransactionSummaries(List<Transaction> transactions);
    }
}