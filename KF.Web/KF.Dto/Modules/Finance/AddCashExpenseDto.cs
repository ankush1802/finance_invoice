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
    public class AddCashExpenseDto : ApiResponseDto
    {
        [DataMember]
        public int BankId { get; set; }
        [DataMember]
        public string ImageName { get; set; }
        [DataMember]
        public string CloudImageName { get; set; }
        [DataMember]
        public bool IsCloud { get; set; }
        [DataMember]
        public string AccountNumber { get; set; }
        [DataMember]
        public string AccountName { get; set; }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string CashBillDate { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Comment { get; set; }
        public string Date { get; set; }
        [DataMember]
        public Nullable<decimal> Credit { get; set; }
        [DataMember]
        public Nullable<decimal> Debit { get; set; }
        [DataMember]
        public Nullable<decimal> Total { get; set; }
        [DataMember]
        public Nullable<int> ClassificationId { get; set; }
        [DataMember]
        public string Purpose { get; set; }
        [DataMember]
        public string Vendor { get; set; }
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public string BillPath { get; set; }
        [DataMember]
        public Nullable<decimal> BillTax { get; set; }
        [DataMember]
        public Nullable<decimal> BillTotal { get; set; }
        [DataMember]
        public float ActualPercentage { get; set; }
        [DataMember]
        public Nullable<decimal> GSTtax { get; set; }
        [DataMember]
        public Nullable<decimal> QSTtax { get; set; }
        [DataMember]
        public Nullable<decimal> HSTtax { get; set; }
        [DataMember]
        public Nullable<decimal> PSTtax { get; set; }
        [DataMember]
        public string ImagePath { get; set; }

    }
    public class UploadedBillsDto
    {
        public string Bills { get; set; }
    }
}
