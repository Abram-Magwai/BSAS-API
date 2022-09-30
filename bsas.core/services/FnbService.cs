using System.Text;
using bsas.core.constants;
using bsas.core.models;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace bsas.core.services
{
    public interface IFnbService
    {
        public List<Transaction> GetTransactions(byte[] fileBytes);
        List<TransactionSummary> GetTransactionSummaries(List<Transaction> transactions);
    }
    public class FnbService : IFnbService
    {
        public List<Transaction> GetTransactions(byte[] fileBytes)
        {
            List<Transaction> transactionList = new();
            using (PdfReader reader = new PdfReader(fileBytes))
            {
                for (int pageNo = 1; pageNo <= reader.NumberOfPages; pageNo++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string text = PdfTextExtractor.GetTextFromPage(reader, pageNo, strategy);
                    text = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(text)));
                    string[] formated = text.Split("\n");
                    List<String> months = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                    for (int transaction = 0; transaction < formated.Length - 1; transaction++)
                    {
                        string[] info = formated[transaction].Split(" ");
                        if (info.Length > 1)
                        {
                            if (months.Contains(info[1]))
                            {
                                string date = info[0] + " " + info[1];
                                string description = info[3] == "Purchase" ? info[4] + " " + info[5] + " " + info[6] : info[3] + " " + info[4];
                                string transactionType = string.Empty;
                                double amount;
                                double balance;
                                int amountController = info[info.Count() - 1].Contains("C") ? 2 : 3;
                                int balanceController = info[info.Count() - 1].Contains("C") ? 1 : 2;
                                if (info[info.Count() - amountController].Contains("C"))
                                {
                                    transactionType = TransactionTypes.Income;
                                    string strAmount = info[info.Count() - amountController];
                                    amount = double.Parse(strAmount.Remove(strAmount.Length - 2).Replace(",", ""));
                                }
                                else
                                {
                                    transactionType = TransactionTypes.Expense;
                                    amount = double.Parse(info[info.Count() - amountController].Replace(",", ""));
                                }

                                if (info[info.Count() - balanceController].Contains("C"))
                                {
                                    string strBalance = info[info.Count() - balanceController];
                                    balance = double.Parse(strBalance.Remove(strBalance.Length - 2).Replace(",", ""));
                                }
                                else
                                {
                                    balance = double.Parse(info[info.Count() - balanceController].Replace(",", ""));
                                }
                                transactionList.Add(new Transaction
                                {
                                    Date = date,
                                    Description = description,
                                    TransactionType = transactionType,
                                    Amount = amount,
                                    Balance = balance
                                });
                            }
                        }
                    }
                }
            }

            return transactionList;
        }
        public List<TransactionSummary> GetTransactionSummaries(List<Transaction> transactions)
        {
            List<TransactionSummary> transactionSummaries = new();

            transactions.ForEach(tr =>
            {
                if (transactionSummaries.Count() == 0)
                {
                    transactionSummaries.Add(new TransactionSummary { Description = tr.Description ?? "", Total = tr.Amount });
                }
                else
                {
                    for (int trs = 0; trs < transactionSummaries.Count(); trs++)
                    {
                        if (transactionSummaries[trs].Description == tr.Description)
                        {
                            transactionSummaries[trs].Total += tr.Amount;
                            transactionSummaries[trs].Visits++;
                            break;
                        }
                        else if (trs == transactionSummaries.Count() - 1)
                        {
                            transactionSummaries.Add(new TransactionSummary { Description = tr.Description ?? "", Total = tr.Amount });
                            break;
                        }
                    }

                }
            });
            return transactionSummaries;
        }
    }
}