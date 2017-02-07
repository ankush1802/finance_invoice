using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Finance
{
    public class AddMjvEntryDto
    {
        public string FiscalYear { get; set; }
        public bool IsVirtualEntry { get; set; }
        public int ReferenceId { get; set; }
        public int BankId { get; set; }
        public string ImageName { get; set; }
        public string CloudImageName { get; set; }
        public bool IsCloud { get; set; }
        public string AccountName { get; set; }
        public string CustomError { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CashBillDate { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public string Date { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<int> ClassificationId { get; set; }
        public string Classification { get; set; }
        public string Status { get; set; }
        public string TransactionType { get; set; }
        public string Purpose { get; set; }
        public string Category { get; set; }
        public string Vendor { get; set; }
        public string customDate { get; set; }
        public int CategoryId { get; set; }
        public string BillPath { get; set; }

        public Nullable<decimal> BillTax { get; set; }

        public Nullable<decimal> BillTotal { get; set; }

        public Nullable<decimal> GSTtax { get; set; }

        public Nullable<decimal> QSTtax { get; set; }

        public Nullable<decimal> HSTtax { get; set; }

        public Nullable<decimal> PSTtax { get; set; }

        public float ActualPercentage { get; set; }
        public List<string> objManualEntryBills { get; set; }
        public string ImagePath { get; set; }
    }
}
