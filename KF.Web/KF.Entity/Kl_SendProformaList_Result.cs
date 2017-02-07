//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KF.Entity
{
    using System;
    
    public partial class Kl_SendProformaList_Result
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string PaymentTerms { get; set; }
        public string DueDate { get; set; }
        public string DocumentRef { get; set; }
        public string SalesPerson { get; set; }
        public Nullable<decimal> ShippingCost { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> DepositePayment { get; set; }
        public Nullable<decimal> BalanceDue { get; set; }
        public string Note { get; set; }
        public string Terms { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public string InvoiceDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string Pro_Status { get; set; }
        public string Pro_FlowStatus { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<bool> IsInvoiceReport { get; set; }
        public string In_R_Status { get; set; }
        public string In_R_FlowStatus { get; set; }
        public string TaxType { get; set; }
        public Nullable<bool> IsSupplierManualPaid { get; set; }
        public Nullable<decimal> SupplierManualPaidAmount { get; set; }
        public string SupplierManualPaidJVID { get; set; }
        public Nullable<bool> IsCustomerManualPaid { get; set; }
        public Nullable<decimal> CustomerManualPaidAmount { get; set; }
        public string CustomerManualPaidJVID { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public string StripeJVID { get; set; }
        public string InvoiceJVID { get; set; }
        public string CustomerInvoiceJVID { get; set; }
    }
}
