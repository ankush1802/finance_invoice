using KF.Dto.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Finance
{
    [DataContract]
    public class BankDto : ApiResponseDto
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
        public Nullable<double> GSTPercentage { get; set; }
         [DataMember]
        public Nullable<decimal> GSTtax { get; set; }
         [DataMember]
        public Nullable<double> QSTPercentage { get; set; }
         [DataMember]
        public Nullable<decimal> QSTtax { get; set; }
         [DataMember]
        public Nullable<double> HSTPercentage { get; set; }
         [DataMember]
        public Nullable<decimal> HSTtax { get; set; }
         [DataMember]
        public Nullable<double> PSTPercentage { get; set; }
         [DataMember]
        public Nullable<decimal> PSTtax { get; set; }
        //[DataMember]
        //public OCRExpenseDto BillData { get; set; }
        //[DataMember]
        //public List<OCRExpenseDto> OcrDataList { get; set; }
    }

    public class ReadScotiaBankDetails
    {
        public Nullable<int> UserId { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> ClassificationId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Credit { get; set; }
        public string Debit { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
    }

    public class ReadBmoBankDetails
    {
        public Nullable<int> UserId { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> ClassificationId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Credit { get; set; }
        public string Debit { get; set; }
        public string Description { get; set; }
        public string TransactionType { get; set; }
        public string AccountNo { get; set; }
    }

    public class ReadTdDetails
    {
        public Nullable<int> UserId { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> ClassificationId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Withdrawal { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string Balance { get; set; }
    }
    public class ReadCisvDetails
    {
        public Nullable<int> UserId { get; set; }
        public Nullable<int> BankId { get; set; }
        public Nullable<int> ClassificationId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Transaction { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
    }
}
