using System.Text;
using bsas.core.constants;
using bsas.core.interfaces;
using bsas.core.models;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace bsas.core.services
{
    public class FnbService : IFnbService
    {
        public static bool DocumentIsValid(string[] formated, int pageNo)
        {
            if (pageNo > 1) return true;
            if (formated.Length > 21)
            {
                if ((formated[21].Split(" ")).Length > 1 && (formated[21].Split(" "))[1] == "fnb.co.za")
                    return true;
            }
            return false;
        }
        public StatementDetails GetStatementDetails(byte[] fileBytes)
        {
            StatementDetails statementDetails = new();
            List<Transaction> transactionList = new();
            using (PdfReader reader = new PdfReader(fileBytes))
            {
                for (int pageNo = 1; pageNo <= reader.NumberOfPages; pageNo++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string text = PdfTextExtractor.GetTextFromPage(reader, pageNo, strategy);
                    text = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(text)));
                    string[] formated = text.Split("\n");
                    if (!DocumentIsValid(formated, pageNo))
                        return new();
                    List<String> months = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                    statementDetails.StatementNumber = pageNo == 1 ? int.Parse(formated[27].Split(":")[1]) : statementDetails.StatementNumber;
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
            statementDetails.Transactions = transactionList;
            return statementDetails;
        }
        public List<TransactionSummary> GetTransactionSummaries(List<Transaction> transactions)
        {
            List<TransactionSummary> transactionSummaries = new();

            transactions.ForEach(transaction =>
            {
                if (transactionSummaries.Count() == 0)
                {
                    transactionSummaries.Add(new TransactionSummary { Description = transaction.Description ?? "", Total = transaction.Amount, TransactionType = transaction.TransactionType });
                }
                else
                {
                    for (int index = 0; index < transactionSummaries.Count(); index++)
                    {
                        if (transactionSummaries[index].Description == transaction.Description)
                        {
                            transactionSummaries[index].Total += transaction.Amount;
                            transactionSummaries[index].TransactionType = transaction.TransactionType;
                            transactionSummaries[index].Visits++;
                            break;
                        }
                        else if (index == transactionSummaries.Count() - 1)
                        {
                            transactionSummaries.Add(new TransactionSummary { Description = transaction.Description ?? "", Total = transaction.Amount, TransactionType = transaction.TransactionType });
                            break;
                        }
                    }

                }
            });
            return transactionSummaries;
        }
        public List<TransactionSummary> Compare(List<TransactionSummary> first, List<TransactionSummary> second)
        {
            List<TransactionSummary> commonDescriptions = new();
            int length = first.Count < second.Count ? first.Count : second.Count;
            for (int i = 0; i < length; i++)
            {
                bool exists = false;
                for (int a = 0; a < (length == first.Count ? second.Count : first.Count); a++)
                {
                    if (first[i].Description == second[a].Description)
                    {
                        exists = !exists;
                        break;
                    }
                }
                if (exists)
                    commonDescriptions.Add(first[i]);
            }
            return commonDescriptions;
        }
    }
}