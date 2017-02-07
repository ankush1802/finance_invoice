using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.FinanceReport
{
    public class TrialBalanceDto
    {
        public Nullable<int> ClassificationID { get; set; }
        public string ClassificationName { get; set; }
        public int ChartAccountNumber { get; set; }
        //public int AccountClassificationNumber { get; set; }
        public String DisplayChartAccountNumber { get; set; }
        public List<TrialBalanceRowItems> objrowItemList { get; set; }
        public decimal ClassificationDebitTotal { get; set; }
        public decimal ClassificationCreditTotal { get; set; }

        public string GrossDebitTotal { get; set; }
        public string GrossCreditTotal { get; set; }
    }
    public class TrialBalanceRowItems
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public string UploadType { get; set; }
        public string Source { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string TransactionType { get; set; }
        public int JVID { get; set; }
        public Nullable<System.DateTime> StatementDate { get; set; }
    }
    public class TrialBalanceTaxDto
    {
        public Nullable<System.DateTime> StatementDate { get; set; }
        public Nullable<int> StatementId { get; set; }
        public int CategoryId { get; set; }
        public Nullable<int> ClassificationId { get; set; }
        public string ClassificationType { get; set; }
        // public int ChartAccountNumber { get; set; }
        public String ChartAccountNumber { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> TaxTotal { get; set; }
        public string Comments { get; set; }
        // public string MjvTaxComments { get; set; }
        public string BankName { get; set; }
        public string Date { get; set; }
        public string UploadType { get; set; }
        public string TransactionType { get; set; }
    }

}
