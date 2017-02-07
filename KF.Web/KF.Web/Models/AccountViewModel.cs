using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KF.Web.Models
{

    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string ErrorMsg { get; set; }
    }


    /*Invoice Section*/
    public class InvoiceLoginViewModel
    {
        [Required]
        public string EmailTo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string ErrorMsg { get; set; }

    }
    public class RegisterInvoiceViewModel
    {
        public string ProfileImage { get; set; }
        public HttpPostedFileBase File { get; set; }

        public string CompanyLogo { get; set; }

        //[Required(ErrorMessage = "Companyname is required")]
        public string CompanyName { get; set; }


        public string ContactPerson { get; set; }

        //[Required(ErrorMessage = "CorporateAptNo is required")]
        public string CorporateAptNo { get; set; }

        //[Required(ErrorMessage = "CorporateHouseNo is required")]
        public string CorporateHouseNo { get; set; }

        //[Required(ErrorMessage = "CorporateStreet is required")]
        public string CorporateStreet { get; set; }

        //[Required(ErrorMessage = "CorporateCity is required")]
        public string CorporateCity { get; set; }

        // [Required(ErrorMessage = "CorporateState is required")]
        public string CorporateState { get; set; }

        //[Required(ErrorMessage = "CorporatePostalCode is required")]
        public string CorporatePostalCode { get; set; }

        public string hdncorporateaptno { get; set; }
        public string hdncorporatehouseno { get; set; }
        public string hdncorporatestreet { get; set; }
        public string hdncorporatecity { get; set; }
        public string hdncorporatestate { get; set; }
        public string hdncorporatepostalcode { get; set; }

        public string hdnshippingaptno { get; set; }
        public string hdnshippinghouseno { get; set; }
        public string hdnshippingstreet { get; set; }
        public string hdnshippingcity { get; set; }
        public string hdnshippingstate { get; set; }
        public string hdnshippingpostalcode { get; set; }

        public string hdnemailCC { get; set; }
        //public string CorporateAptNo { get; set; }

        public string ShippingAptNo { get; set; }

        public string ShippingHouseNo { get; set; }

        public string ShippingStreet { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingState { get; set; }

        public string ShippingPostalCode { get; set; }

        //[Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }


        public Nullable<byte> GoodsservicesId { get; set; }
        public List<SelectListItem> GoodsservicesList { get; set; }
        public string GoodsType { get; set; }

        //[Required(ErrorMessage = "TradingCurrency is required")]
        public Nullable<byte> TradingCurrencyId { get; set; }
        public List<SelectListItem> TradingCurrencyList { get; set; }
        public string TradingCurrency { get; set; }


        //[Required(ErrorMessage = "BusinessNumber is required")]
        public string BusinessNumber { get; set; }


        //[Required(ErrorMessage = "Email is required")]
        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }


        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Area code must be a numeric.")]
        public string AreaCode { get; set; }

        //[RegularExpression("([0-9][0-9]*)", ErrorMessage = "MobileNumber must be a numeric.")]
        //[StringLength(7, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 7)]
        public string MobileNumber { get; set; }

        // [RegularExpression("([0-9][0-9]*)", ErrorMessage = "Telephone must be a numeric.")]
        //[StringLength(7, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        public string Telephone { get; set; }



        public int RoleId { get; set; }
        public int UserId { get; set; }


        public string Website { get; set; }

        public string Password { get; set; }
        public string ddl { get; set; }
    }

    public class RegisterInvoiceViewModel1
    {
        public string UserName { get; set; }
        public string CompanyLogo { get; set; }


        public string CompanyName { get; set; }


        public string ContactPerson { get; set; }


        public string CorporateAptNo { get; set; }


        public string CorporateHouseNo { get; set; }


        public string CorporateStreet { get; set; }


        public string CorporateCity { get; set; }


        public string CorporateState { get; set; }


        public string CorporatePostalCode { get; set; }


        public string ShippingAptNo { get; set; }

        public string ShippingHouseNo { get; set; }

        public string ShippingStreet { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingState { get; set; }

        public string ShippingPostalCode { get; set; }

        public string GoodsType { get; set; }

        public string TradingCurrency { get; set; }

        public string BusinessNumber { get; set; }


        public string EmailTo { get; set; }
        public string EmailCc { get; set; }


        public string AreaCode { get; set; }

        public string MobileNumber { get; set; }

        public string Website { get; set; }

        public string Password { get; set; }

    }

    public class UpdateCustomer
    {
        public int Id { get; set; }

        public string CustomerAddress { get; set; }
        public string ShippingAddress { get; set; }

        public string CompanyName { get; set; }
        public bool Status { get; set; }



        public string hdncorporateaptno { get; set; }
        public string hdncorporatehouseno { get; set; }
        public string hdncorporatestreet { get; set; }
        public string hdncorporatecity { get; set; }
        public string hdncorporatestate { get; set; }
        public string hdncorporatepostalcode { get; set; }

        public string hdnshippingaptno { get; set; }
        public string hdnshippinghouseno { get; set; }
        public string hdnshippingstreet { get; set; }
        public string hdnshippingcity { get; set; }
        public string hdnshippingstate { get; set; }
        public string hdnshippingpostalcode { get; set; }
        public string hdnemailCC { get; set; }



        public string ContactPerson { get; set; }

        //[Required(ErrorMessage = "CorporateAptNo is required")]
        public string CorporateAptNo { get; set; }

        //[Required(ErrorMessage = "CorporateHouseNo is required")]
        public string CorporateHouseNo { get; set; }

        //[Required(ErrorMessage = "CorporateStreet is required")]
        public string CorporateStreet { get; set; }

        //[Required(ErrorMessage = "CorporateCity is required")]
        public string CorporateCity { get; set; }

        // [Required(ErrorMessage = "CorporateState is required")]
        public string CorporateState { get; set; }

        //[Required(ErrorMessage = "CorporatePostalCode is required")]
        public string CorporatePostalCode { get; set; }


        public string ShippingAptNo { get; set; }

        public string ShippingHouseNo { get; set; }

        public string ShippingStreet { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingState { get; set; }

        public string ShippingPostalCode { get; set; }

        //[Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }


        public Nullable<byte> GoodsservicesId { get; set; }
        public List<SelectListItem> GoodsservicesList { get; set; }
        public string GoodsType { get; set; }

        //[Required(ErrorMessage = "TradingCurrency is required")]
        public Nullable<byte> TradingCurrencyId { get; set; }
        public List<SelectListItem> TradingCurrencyList { get; set; }
        public string TradingCurrency { get; set; }


        //[Required(ErrorMessage = "BusinessNumber is required")]
        public string BusinessNumber { get; set; }


        //[Required(ErrorMessage = "Email is required")]
        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }


        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Area code must be a numeric.")]
        public string AreaCode { get; set; }

        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "MobileNumber must be a numeric.")]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 7)]
        public string MobileNumber { get; set; }

        public string Website { get; set; }



    }
    public class UpdateSupplier
    {
        public int Id { get; set; }
        public string SupplierAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string CompanyName { get; set; }
        public bool Status { get; set; }
        public string hdnshippingaptno { get; set; }
        public string hdnshippinghouseno { get; set; }
        public string hdnshippingstreet { get; set; }
        public string hdnshippingcity { get; set; }
        public string hdnshippingstate { get; set; }
        public string hdnshippingpostalcode { get; set; }
        public string hdnemailCC { get; set; }

        public string ServiceOffered { get; set; }

        public string ContactPerson { get; set; }


        public string ShippingAptNo { get; set; }

        public string ShippingHouseNo { get; set; }

        public string ShippingStreet { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingState { get; set; }

        public string ShippingPostalCode { get; set; }

        //[Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }


        public Nullable<int> GoodsservicesId { get; set; }
        public List<SelectListItem> GoodsservicesList { get; set; }
        public string GoodsType { get; set; }

        //[Required(ErrorMessage = "TradingCurrency is required")]
        public Nullable<byte> TradingCurrencyId { get; set; }
        public List<SelectListItem> TradingCurrencyList { get; set; }
        public string TradingCurrency { get; set; }


        //[Required(ErrorMessage = "BusinessNumber is required")]
        public string BusinessNumber { get; set; }


        //[Required(ErrorMessage = "Email is required")]
        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }


        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Area code must be a numeric.")]
        public string AreaCode { get; set; }

        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "MobileNumber must be a numeric.")]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 7)]
        public string MobileNumber { get; set; }

        public string Website { get; set; }



    }

    public class CreateSupplier
    {
        public string AdditionalEmail { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Company_Name { get; set; }
        public string Credetor { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModify { get; set; }
        public string Debtors { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public int Id { get; set; }
        public bool? Isdeleted { get; set; }
        public bool? IsEmailExist { get; set; }
        public bool? IsFinance { get; set; }
        public string Mobile { get; set; }
        public string PostalCode { get; set; }
        public int? RoleId { get; set; }
        public string ServiceOffered { get; set; }
        public string ShippingAptNo { get; set; }
        public string ShippingHouseNo { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingState { get; set; }
        public string ShippingStreet { get; set; }
        public string State { get; set; }
        public string Telephone { get; set; }
        public int? UserId { get; set; }
        public string Website { get; set; }
        public string hdnshippingaptno { get; set; }
        public string hdnshippinghouseno { get; set; }
        public string hdnshippingstreet { get; set; }
        public string hdnshippingcity { get; set; }
        public string hdnshippingstate { get; set; }
        public string hdnshippingpostalcode { get; set; }
        public string hdnemailCC { get; set; }
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Area code must be a numeric.")]
        public string AreaCode { get; set; }

        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "MobileNumber must be a numeric.")]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 7)]
        public string MobileNumber { get; set; }

        public Nullable<int> GoodsservicesId { get; set; }
        public List<SelectListItem> GoodsservicesList { get; set; }
        public string GoodsType { get; set; }

        public string EmailCc { get; set; }

    }
    public class CreateCustomer
    {
        public string AdditionalEmail { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Company_Name { get; set; }
        public string Credetor { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModify { get; set; }
        public string Debtors { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public int Id { get; set; }
        public bool? Isdeleted { get; set; }
        public bool? IsEmailExist { get; set; }
        public bool? IsFinance { get; set; }
        public string Mobile { get; set; }
        public string PostalCode { get; set; }
        public int? RoleId { get; set; }
        public string ServiceOffered { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingState { get; set; }
        public string State { get; set; }
        public string Telephone { get; set; }
        public int UserId { get; set; }
        public string Website { get; set; }

        public string hdncorporateaptno { get; set; }
        public string hdncorporatehouseno { get; set; }
        public string hdncorporatestreet { get; set; }
        public string hdncorporatecity { get; set; }
        public string hdncorporatestate { get; set; }
        public string hdncorporatepostalcode { get; set; }

        public string hdnshippingaptno { get; set; }
        public string hdnshippinghouseno { get; set; }
        public string hdnshippingstreet { get; set; }
        public string hdnshippingcity { get; set; }
        public string hdnshippingstate { get; set; }
        public string hdnshippingpostalcode { get; set; }
        public string CorporateAptNo { get; set; }

        public string CorporateHouseNo { get; set; }

        public string CorporateStreet { get; set; }

        public string CorporateCity { get; set; }

        public string CorporateState { get; set; }

        public string CorporatePostalCode { get; set; }


        public string ShippingAptNo { get; set; }

        public string ShippingHouseNo { get; set; }

        public string ShippingStreet { get; set; }
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Area code must be a numeric.")]
        public string AreaCode { get; set; }

        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "MobileNumber must be a numeric.")]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 7)]
        public string MobileNumber { get; set; }
        public string EmailCc { get; set; }
        public string hdnemailCC { get; set; }


    }
    public class CreateCustomerSave
    {

        public int Id { get; set; }

        public string Company_Name { get; set; }

        public string FirstName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string ServiceOffered { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public Nullable<int> UserId { get; set; }

        public Nullable<System.DateTime> DateCreated { get; set; }

        public Nullable<System.DateTime> DateModify { get; set; }

        public Nullable<bool> Isdeleted { get; set; }

        public Nullable<int> RoleId { get; set; }

        public string AdditionalEmail { get; set; }

        public string ShippingAddress { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingState { get; set; }

        public string ShippingPostalCode { get; set; }

        public Nullable<bool> IsFinance { get; set; }

        public Nullable<bool> IsEmailExist { get; set; }

        public string Telephone { get; set; }

        public string Debtors { get; set; }

        public string Credetor { get; set; }

    }
    public class CreateSupplierSave
    {
        public string Company_Name { get; set; }
        public string FirstName { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

        public string Mobile { get; set; }
        public string Email { get; set; }
        public string ServiceOffered { get; set; }

        public string Website { get; set; }
        public string Telephone { get; set; }
        public int? RoleId { get; set; }
        public int? UserId { get; set; }

    }

    public class CheckInvoice
    {

        public int Id { get; set; }

        public string Company_Name { get; set; }

        public string FirstName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string ServiceOffered { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public Nullable<int> UserId { get; set; }

        public Nullable<System.DateTime> DateCreated { get; set; }

        public Nullable<System.DateTime> DateModify { get; set; }

        public Nullable<bool> Isdeleted { get; set; }

        public Nullable<int> RoleId { get; set; }

        public string AdditionalEmail { get; set; }

        public string ShippingAddress { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingState { get; set; }

        public string ShippingPostalCode { get; set; }

        public Nullable<bool> IsFinance { get; set; }

        public Nullable<bool> IsEmailExist { get; set; }

        public string Telephone { get; set; }

        public string Debtors { get; set; }

        public string Credetor { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
    }

    public class CheckSupplier
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public string EmailTo { get; set; }
        public string Email { get; set; }
    }

    public class CreateInvoice
    {
        public List<decimal> Amount { get; set; }
        public decimal? BalanceDue { get; set; }
        public string ButtonType { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<string> Customer_Service { get; set; }
        public List<int> Customer_ServiceTypeId { get; set; }
        public int? CustomerId { get; set; }
        public decimal? DepositePayment { get; set; }
        public List<string> Description { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#.#}")]
        public List<string> Discount { get; set; }
        public string DocumentRef { get; set; }
        public string DueDate { get; set; }
        public List<string> GST_Tax { get; set; }
        public List<string> HST_Tax { get; set; }
        public int Id { get; set; }
        public string In_R_FlowStatus { get; set; }
        public string In_R_Status { get; set; }
        public string InvoiceDate { get; set; }
        public int? InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsInvoiceReport { get; set; }
        public List<string> Item { get; set; }
        public List<int> ItemId { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Note { get; set; }
        public string PaymentTerms { get; set; }
        public string Pro_FlowStatus { get; set; }
        public string Pro_Status { get; set; }
        public List<string> PST_Tax { get; set; }
        public List<string> QST_Tax { get; set; }
        public List<int> Quantity { get; set; }
        public List<decimal> Rate { get; set; }
        public int? RoleId { get; set; }
        public string SalesPerson { get; set; }
        public string SectionType { get; set; }
        // public List<string> ServiceType { get; set; }
        public List<SelectListItem> ServiceType { get; set; }

        public List<int> ServiceTypeId { get; set; }
        public decimal? ShippingCost { get; set; }
        public List<decimal> SubTotal { get; set; }
        public List<string> Tax { get; set; }
        public string TaxType { get; set; }
        public string Terms { get; set; }
        public decimal? Total { get; set; }
        public int? Type { get; set; }
        public int? UserId { get; set; }
        public int? hdnselectuser { get; set; }


    }

    public class InvoiceReport
    {
        public string CustomerManualPaidJVID { get; set; }
        public int? CustomerManualPaidAmount { get; set; }
        public int? InvoiceId { get; set; }
        public bool? IsCustomer { get; set; }

    }

}