using KF.Dto.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Invoice
{
    [DataContract]
    public partial class BankDto : ApiResponseDto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public int BankId { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public decimal Credit { get; set; }
        [DataMember]
        public decimal Debit { get; set; }
        [DataMember]
        public decimal Total { get; set; }
        [DataMember]
        public string AccountType { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public bool IsAccepted { get; set; }
        [DataMember]
        public bool IsSaved { get; set; }
        [DataMember]
        public string Expense { get; set; }
        [DataMember]
        public int ExpenseId { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public int StatusId { get; set; }
        [DataMember]
        public string TransactionType { get; set; }
        [DataMember]
        public string Purpose { get; set; }
        [DataMember]
        public string Vendor { get; set; }
        [DataMember]
        public string Category { get; set; }
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public string Comments { get; set; }
        [DataMember]
        public int ClassificationId { get; set; }
        [DataMember]
        public string ClassificationType { get; set; }
        [DataMember]
        public string ClassificationDescription { get; set; }
        [DataMember]
        public decimal BillTotal { get; set; }
        [DataMember]
        public decimal BillTax { get; set; }
        [DataMember]
        public Nullable<decimal> GSTtax { get; set; }
        [DataMember]
        public Nullable<decimal> QSTtax { get; set; }
        [DataMember]
        public Nullable<decimal> HSTtax { get; set; }
        [DataMember]
        public Nullable<decimal> PSTtax { get; set; }
        //[DataMember]
        //public OCRExpenseDto BillData { get; set; }
        //[DataMember]
        //public List<OCRExpenseDto> OcrDataList { get; set; }
    }
}
