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
            return _fnbService.GetTransactions(statementFileBytes);
        }
        return new List<Transaction>(){new Transaction{Description = "no transactions"}};
    }
    [HttpPost("transactions/summary")]
    public List<TransactionSummary> GetTransactionSummaries(string bankName, IFormFile statement)
    {
        if (!FileHelper.IsPdf(statement.FileName))
            throw new Exception(ErrorMessage.NotPDf);

        Stream fileStream = statement.OpenReadStream();
        var statementFileBytes = FileHelper.streamToByteArray(fileStream);

        if (bankName.ToUpper() == BankNameConstants.FNB)
            return _fnbService.GetTransactionSummaries(_fnbService.GetTransactions(statementFileBytes));
        return new List<TransactionSummary>();
    }
}