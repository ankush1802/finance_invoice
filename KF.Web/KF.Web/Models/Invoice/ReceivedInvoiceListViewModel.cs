using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KF.Web.Models
{
    public class ReceivedInvoiceListViewModel
    {
        public decimal? BalanceDue { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CustomerId { get; set; }
        public decimal? CustomerManualPaidAmount { get; set; }
        public string CustomerManualPaidJVID { get; set; }
        public decimal? DepositePayment { get; set; }
        public string DocumentRef { get; set; }
        public string DueDate { get; set; }
        public string FirstName { get; set; }
        public int Id { get; set; }
        public string In_R_FlowStatus { get; set; }
        public string In_R_Status { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public bool IsCustomer { get; set; }
        public bool? IsCustomerManualPaid { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsInvoiceReport { get; set; }
        public bool? IsSupplierManualPaid { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Note { get; set; }
        public string PaymentTerms { get; set; }
        public string Pro_FlowStatus { get; set; }
        public string Pro_Status { get; set; }
        public int? RoleId { get; set; }
        public string SalesPerson { get; set; }
        public decimal? ShippingCost { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? SupplierManualPaidAmount { get; set; }
        public string SupplierManualPaidJVID { get; set; }
        public string Tax { get; set; }
        public string Terms { get; set; }
        //public decimal? Total { get; set; }
        public String Total { get; set; }
        public int? Type { get; set; }
        public String StrType { get; set; }
        public int? UserId { get; set; }
        public string Username { get; set; }
        public string ButtonType { get; set; }
        public string SectionType { get; set; }
        public int ItemsCount { get; set; }

        //public int userid { get; set; }
        public List<string> Customer_Service { get; set; }
        public List<int> Customer_ServiceTypeId { get; set; }
        public List<string> ServiceType { get; set; }
        public List<int> ServiceTypeId { get; set; }
        public List<string> Item { get; set; }
        public List<int> ItemId { get; set; }
        public String PdfViewPath { get; set; }

        public bool IsStripe { get; set; }
        public string PaymentDate { get; set; }

        public string StripeJVID { get; set; }
        public string InvoiceJVID { get; set; }
        public string MethodOfPayment { get; set; }

        //Bank Type List
        public string selectedStatementTypeId { get; set; }
        public List<SelectListItem> StatementTypeList { get; set; }
       // Bank Type List
        public string selectedStatementTypeId2 { get; set; }
        public List<SelectListItem> StatementTypeList2 { get; set; }

    }
    public class ReceivedInvoiceListViewModel1
    {
        public decimal? BalanceDue { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CustomerId { get; set; }
        public decimal? CustomerManualPaidAmount { get; set; }
        public string CustomerManualPaidJVID { get; set; }
        public decimal? DepositePayment { get; set; }
        public string DocumentRef { get; set; }
        public string DueDate { get; set; }
        public string FirstName { get; set; }
        public int Id { get; set; }
        public string In_R_FlowStatus { get; set; }
        public string In_R_Status { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public bool IsCustomer { get; set; }
        public bool? IsCustomerManualPaid { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsInvoiceReport { get; set; }
        public bool? IsSupplierManualPaid { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Note { get; set; }
        public string PaymentTerms { get; set; }
        public string Pro_FlowStatus { get; set; }
        public string Pro_Status { get; set; }
        public int? RoleId { get; set; }
        public string SalesPerson { get; set; }
        public decimal? ShippingCost { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? SupplierManualPaidAmount { get; set; }
        public string SupplierManualPaidJVID { get; set; }
        public string Tax { get; set; }
        public string Terms { get; set; }
        //public decimal? Total { get; set; }
        public String Total { get; set; }
        public int? Type { get; set; }
        public String StrType { get; set; }
        public int? UserId { get; set; }
        public string Username { get; set; }
        public string ButtonType { get; set; }
        public string SectionType { get; set; }
        public int ItemsCount { get; set; }
        //public int userid { get; set; }
        public List<string> Customer_Service { get; set; }
        public List<int> Customer_ServiceTypeId { get; set; }
        public List<string> ServiceType { get; set; }
        public List<int> ServiceTypeId { get; set; }
        public List<string> Item { get; set; }
        public List<int> ItemId { get; set; }
        public String PdfViewPath { get; set; }

        public bool IsStripe { get; set; }

        public string PaymentDate { get; set; }
        public string StripeJVID { get; set; }
        public string InvoiceJVID { get; set; }
        public string MethodOfPayment { get; set; }

    }
}