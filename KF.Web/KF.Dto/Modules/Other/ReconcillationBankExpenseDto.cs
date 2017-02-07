using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Other
{
    public class ReconcillationBankExpenseDto
    {
        public Nullable<int> year { get; set; }
        public decimal directorval { get; set; }
        public string Bank { get; set; } //BankName
        public int StatementId { get; set; }
        public int BankId { get; set; }
        public int TotalCount { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Total { get; set; }
        //[Required(ErrorMessage = "Classification is required")]
        public Nullable<int> ClassificationId { get; set; }
        public string Classification { get; set; }
        public string Expense { get; set; }
        public int ExpenseId { get; set; }
        public string Status { get; set; }
        public string TransactionType { get; set; }
        public string Purpose { get; set; }
        public string Category { get; set; }
        public string Vendor { get; set; }
        //  GroupedSelectListItem
        public string ClassificationDescription { get; set; }
        public int CategoryId { get; set; }
        public string customDate { get; set; }
        public int PrevRecId { get; set; }
        public int NextRecId { get; set; }
        public decimal OcrCashTotalForCash { get; set; }
        public decimal OcrBankTotal { get; set; }
        public string UploadType { get; set; }
        public string hdnSelectedImagePath { get; set; }
        public int AccountClassificationId { get; set; }
        public List<string> ObjBillImagesList { get; set; }
        public decimal TotalBalance { get; set; }

    }
}
