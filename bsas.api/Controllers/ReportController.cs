using bsas.api.Controllers;
using bsas.core.constants;
using bsas.core.helper;
using bsas.core.interfaces;
using bsas.core.models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ReportController : ControllerBase
{
    IFnbService _fnbService;
    public ReportController(IFnbService fnbService)
    {
        _fnbService = fnbService;
    }

    [HttpPost("transactions")]
    public List<Transaction> GetTransactions(string bankName, IFormFile statement)
    {
        if (!FileHelper.IsPdf(statement.FileName))
            throw new Exception(ErrorMessage.NotPDf);

        Stream fileStream = statement.OpenReadStream();
        var statementFileBytes = FileHelper.streamToByteArray(fileStream);

        if (bankName.ToUpper() == BankNameConstants.FNB)
        {
            return _fnbService.GetStatementDetails(statementFileBytes).Transactions;
        }
        return new List<Transaction>() {new Transaction { Description = "no transactions" } };
    }
    [HttpPost("transactions/summary")]
    public List<TransactionSummary> GetTransactionSummaries(string bankName, IFormFile statement)
    {
        if (!FileHelper.IsPdf(statement.FileName))
            throw new Exception(ErrorMessage.NotPDf);

        Stream fileStream = statement.OpenReadStream();
        var statementFileBytes = FileHelper.streamToByteArray(fileStream);

        if (bankName.ToUpper() == BankNameConstants.FNB)
        {
            var check = _fnbService.GetTransactionSummaries(_fnbService.GetStatementDetails(statementFileBytes).Transactions);
            return check;
        }
        return new List<TransactionSummary>();
    }
    [HttpPost("transactions/common")]
    public List<CommonTransaction> GetCommonTransactions(List<IFormFile> statements)
    {
        List<StatementDetails> statementDetailsList = new();
        StatementDetails statementDetails = new();

        foreach (var file in statements)
        {
            Stream fileStream = file.OpenReadStream();
            var statementFileBytes = FileHelper.streamToByteArray(fileStream);
            statementDetails = _fnbService.GetStatementDetails(statementFileBytes);
            statementDetailsList.Add(statementDetails);
        }
        statementDetailsList.Sort();
        List<List<TransactionSummary>> transactionSummariesList = new();
        foreach (var statement in statementDetailsList)
            transactionSummariesList.Add(_fnbService.GetTransactionSummaries(statement.Transactions));
        List<TransactionSummary> commonTransactionSummaries = new();
        for (int i = 0; i < transactionSummariesList.Count - 1; i++)
        {
            commonTransactionSummaries = _fnbService.Compare(commonTransactionSummaries.Count == 0 ? transactionSummariesList[i] : commonTransactionSummaries, transactionSummariesList[i + 1]);
        }
        List<CommonTransaction> CommonTransactions = new();
        foreach (var com in commonTransactionSummaries)
        {
            CommonTransaction commonTransaction = new CommonTransaction();
            foreach (var StatementSummaries in transactionSummariesList)
            {
                var transactionSummary = StatementSummaries.First(x => x.Description == com.Description);
                commonTransaction.Description = transactionSummary.Description??"";
                commonTransaction.TransactionType = transactionSummary.TransactionType??"";
                commonTransaction.Maximum = transactionSummary.Total > commonTransaction.Maximum ? transactionSummary.Total : commonTransaction.Maximum;
                commonTransaction.Minimum = transactionSummary.Total < commonTransaction.Minimum ? transactionSummary.Total : commonTransaction.Minimum;
            }
            CommonTransactions.Add(commonTransaction);
        }
        return CommonTransactions;
    }
}