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
    public partial class InvoiceDetailDto : ApiResponseDto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string ButtonType { get; set; }
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
        public string InvoiceDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifyDate { get; set; }
        [DataMember]
        public Nullable<bool> IsDeleted { get; set; }
        [DataMember]
        public Nullable<int> UserId { get; set; }
        [DataMember]
        public Nullable<int> CustomerId { get; set; }
        [DataMember]
        public Nullable<int> RoleId { get; set; }
        [DataMember]
        public List<string> ServiceType { get; set; }
        [DataMember]
        public List<string> Description { get; set; }
        [DataMember]
        public List<int> Quantity { get; set; }
        [DataMember]
        public List<decimal> Rate { get; set; }
        [DataMember]
        public List<decimal> Amount { get; set; }
        [DataMember]
        public List<string> Item { get; set; }
        [DataMember]
        public Nullable<int> InvoiceId { get; set; }
        [DataMember]
        public List<string> Discount { get; set; }
        [DataMember]
        public List<int> ServiceTypeId { get; set; }
        [DataMember]
        public List<int> ItemId { get; set; }
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
        public List<int> Customer_ServiceTypeId { get; set; }
        [DataMember]
        public List<string> Customer_Service { get; set; }
        [DataMember]
        public List<string> Tax { get; set; }
        [DataMember]
        public List<string> GST_Tax { get; set; }
        [DataMember]
        public List<string> HST_Tax { get; set; }
        [DataMember]
        public List<string> PST_Tax { get; set; }
        [DataMember]
        public List<string> QST_Tax { get; set; }
        [DataMember]
        public string TaxType { get; set; }
        [DataMember]
        public string SectionType { get; set; }
        [DataMember]
        public List<decimal> SubTotal { get; set; }

    }
}
