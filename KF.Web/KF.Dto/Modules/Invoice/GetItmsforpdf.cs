using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Invoice
{
     [DataContract]
    public  class GetItmsforpdf
    {
         [DataMember]
        public int Id { get; set; }
           [DataMember]
        public string ServiceType { get; set; }
           [DataMember]
        public string Description { get; set; }
           [DataMember]
        public Nullable<int> Quantity { get; set; }
           [DataMember]
        public Nullable<decimal> Rate { get; set; }
           [DataMember]
        public Nullable<decimal> Amount { get; set; }
           [DataMember]
        public string Item { get; set; }
           [DataMember]
        public Nullable<int> InvoiceId { get; set; }
           [DataMember]
        public string Discount { get; set; }
           [DataMember]
        public Nullable<int> ServiceTypeId { get; set; }
           [DataMember]
        public Nullable<int> ItemId { get; set; }
           [DataMember]
        public Nullable<int> Customer_ServiceTypeId { get; set; }
           [DataMember]
        public string Customer_Service { get; set; }
           [DataMember]
        public string Tax { get; set; }
           [DataMember]
        public string GST_Tax { get; set; }
           [DataMember]
        public string HST_Tax { get; set; }
           [DataMember]
        public string PST_Tax { get; set; }
           [DataMember]
        public string QST_Tax { get; set; }
           [DataMember]
        public Nullable<decimal> SubTotal { get; set; }
           [DataMember]
        public string DiscountAmount { get; set; }
           [DataMember]
        public string DiscountPercentage { get; set; }
           [DataMember]
        public string TaxAmount { get; set; }
           [DataMember]
        public string TaxPercentage { get; set; }
           [DataMember]
        public string GST_TaxAmount { get; set; }
           [DataMember]
        public string GST_TaxPercentage { get; set; }
           [DataMember]
        public string HST_TaxAmount { get; set; }
           [DataMember]
        public string HST_TaxPercentage { get; set; }
           [DataMember]
        public string PST_TaxAmount { get; set; }
           [DataMember]
        public string PST_TaxPercentage { get; set; }
           [DataMember]
        public string QST_TaxAmount { get; set; }
           [DataMember]
        public string QST_TaxPercentage { get; set; }
    }
}
