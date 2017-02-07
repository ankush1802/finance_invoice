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
    public partial class Sp_SendInvoiceListDto : ApiResponseDto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string InvoiceNumber { get; set; }
        [DataMember]
        public string PaymentTerms { get; set; }
        [DataMember]
        public string DueDate { get; set; }
        [DataMember]
        public string DocumentRef { get; set; }
        [DataMember]
        public string SalesPerson { get; set; }
        [DataMember]
        public Nullable<decimal> ShippingCost { get; set; }
        [DataMember]
        public Nullable<decimal> SubTotal { get; set; }
        [DataMember]
        public Nullable<decimal> Total { get; set; }
        [DataMember]
        public Nullable<decimal> DepositePayment { get; set; }
        [DataMember]
        public Nullable<decimal> BalanceDue { get; set; }
        [DataMember]
        public string Note { get; set; }
        [DataMember]
        public string Terms { get; set; }
        [DataMember]
        public Nullable<int> UserId { get; set; }
        [DataMember]
        public Nullable<int> CustomerId { get; set; }
        [DataMember]
        public Nullable<int> RoleId { get; set; }
        [DataMember]
        public string InvoiceDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifyDate { get; set; }
        [DataMember]
        public Nullable<bool> IsDeleted { get; set; }
        [DataMember]
        public string Pro_Status { get; set; }
        [DataMember]
        public string Pro_FlowStatus { get; set; }
        [DataMember]
        public Nullable<int> Type { get; set; }
        [DataMember]
        public Nullable<bool> IsInvoiceReport { get; set; }
        [DataMember]
        public string In_R_Status { get; set; }
        [DataMember]
        public string In_R_FlowStatus { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string Tax { get; set; }
        [DataMember]
        public bool IsCustomer { get; set; }
        [DataMember]
        public Nullable<bool> IsSupplierManualPaid { get; set; }
        [DataMember]
        public Nullable<decimal> SupplierManualPaidAmount { get; set; }
        [DataMember]
        public string SupplierManualPaidJVID { get; set; }
        [DataMember]
        public Nullable<bool> IsCustomerManualPaid { get; set; }
        [DataMember]
        public Nullable<decimal> CustomerManualPaidAmount { get; set; }
        [DataMember]
        public string CustomerManualPaidJVID { get; set; }
        [DataMember]
        public bool IsStripe { get; set; }
        [DataMember]
        public string PaymentDate { get; set; }
        [DataMember]
        public string InvoiceJVID { get; set; }
        [DataMember]
        public bool IsPaymentbyStripe { get; set; }
         [DataMember]
        public string StripeJVID { get; set; }
        
    }
}


