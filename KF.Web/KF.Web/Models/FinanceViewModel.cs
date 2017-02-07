using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KF.Web.Models
{

    public class ChangePasswordViewModel
    {
        // [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Old Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Re-enter your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string DefaultMsg { get; set; }
    }
    public class EditProfile
    {
        public int Id { get; set; }
        public string ProfileImage { get; set; }
        public int RoleId { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Area code is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Area code must be a numeric.")]
        public string AreaCode { get; set; }

        [Required(ErrorMessage = "MobileNumber is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "MobileNumber must be a numeric.")]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 7)]
        public string MobileNumber { get; set; }

        //[Required(ErrorMessage = "CountryId is required")]
        //public int CountryId { get; set; }

        public string Province { get; set; }
        [Required(ErrorMessage = "Province is required")]
        public int ProvinceId { get; set; }
        public List<SelectListItem> ProvinceList { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "PostalCode is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Postal code must be 6 digit long.")]
        public string PostalCode { get; set; }

        //[Required(ErrorMessage = "IndustryId is required")]
        //public int IndustryId { get; set; }

        [Required(ErrorMessage = "CompanyName is required")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "CorporationAddress is required")]
        public string CorporationAddress { get; set; }

        [Required(ErrorMessage = "GSTNumber is required")]
        public string GSTNumber { get; set; }

        [Required(ErrorMessage = "BusinessNumber is required")]
        public string BusinessNumber { get; set; }

        [Required(ErrorMessage = "OwnershipId is required")]
        public int OwnershipId { get; set; }

        [Required(ErrorMessage = "IndustryId is required")]
        public int IndustryId { get; set; }

        [Required(ErrorMessage = "CurrencyId is required")]
        public int CurrencyId { get; set; }

        public bool IsActive { get; set; }

        //public List<Country> CountryList { get; set; }
        public List<KF.Entity.Ownership> OwnershipList { get; set; }
        public List<KF.Entity.Industry> IndustryList { get; set; }
    }

    /// <summary>
    /// List of customer so that while creating an sub-accountant accountant select the users
    /// </summary>
    public class AccountantCustomers
    {
        public int CustomerId
        {
            get;
            set;
        }
        public string CustomerName
        {
            get;
            set;
        }
        public bool IsChecked
        {
            get;
            set;
        }
    }

    public class SubAccountantViewModel
    {
        public string EmployeeStatus { get; set; }
        public string EmailSendStatus { get; set; }
        public string SendMail { get; set; }
        public int Id { get; set; }
        public int? AccountantId { get; set; }
        public int RoleId { get; set; }
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
        public bool Status { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Area code is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Area code must be a numeric.")]
        public string AreaCode { get; set; }

        [Required(ErrorMessage = "MobileNumber is required")]
        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "MobileNumber must be a numeric.")]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 7)]
        public string MobileNumber { get; set; }
        public string AccountantUnderEmployees { get; set; } //Rohin added this

        public List<AccountantCustomers> CustomerList { get; set; }

        public int TotalCount { get; set; }
    }
    public class CatNClassViewModel
    {
        public int Id { get; set; }
        public string CategoryType { get; set; }
        public string ClassificationDesc { get; set; }
        public string ClassificationChartNumber { get; set; }
    }
    public class YodleeAccountLoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string CustomError { get; set; }
    }
    public class RegisterViewModel
    {
        [Display(Name = "Terms and Conditions")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please Accept the terms and condtion")]
        public bool TermsAndConditions { get; set; }

        public int Id { get; set; }
        public Nullable<int> AccountantId { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password Mismatch.")]
        public string ConfirmPassword { get; set; }


        // [Required(ErrorMessage = "Country Code is required")]
        //public Nullable<byte> CountryCodeId { get; set; }
        //public List<SelectListItem> CountryCodeList { get; set; }

        [Required(ErrorMessage = "Area code is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Area code must be a numeric.")]
        public string AreaCode { get; set; }

        [Required(ErrorMessage = "MobileNumber is required")]
        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "MobileNumber must be a numeric.")]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 7)]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "CountryId is required")]
        public int CountryId { get; set; }



        [Required(ErrorMessage = "Province is required")]
        public Nullable<byte> ProvinceId { get; set; }
        public List<SelectListItem> ProvinceList { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "PostalCode is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Postal code must be 6 digit long.")]
        public string PostalCode { get; set; }

        public Nullable<int> SectorId { get; set; }

        //  [Required(ErrorMessage = "IndustryId is required")]
        public Nullable<int> IndustryId { get; set; }

        [Required(ErrorMessage = "CompanyName is required")]
        public string CompanyName { get; set; }
        public string Country1 { get; set; }
        [Required(ErrorMessage = "CorporationAddress is required")]
        public string CorporationAddress { get; set; }

        [Required(ErrorMessage = "GSTNumber is required")]
        public string GSTNumber { get; set; }

        [Required(ErrorMessage = "BusinessNumber is required")]
        public string BusinessNumber { get; set; }

        //  [Required(ErrorMessage = "OwnershipId is required")]
        public int OwnershipId { get; set; }

        // [Required(ErrorMessage = "CurrencyId is required")]
        public int CurrencyId { get; set; }

        [Required(ErrorMessage = "LicenseId is required")]
        public int LicenseId { get; set; }

        public Nullable<int> RoleId { get; set; }

        public string TaxStartYear { get; set; }

        public string TaxEndYear { get; set; }

        public string PrivateKey { get; set; }

        public Nullable<bool> IsCompleted { get; set; }

        public Nullable<byte> TaxStartMonthId { get; set; }

        public Nullable<byte> TaxEndMonthId { get; set; }

        public Nullable<bool> IsTrial { get; set; }

        public Nullable<bool> IsEmailSent { get; set; }

        public string Status { get; set; }

        public Nullable<bool> IsVerified { get; set; }

        public Nullable<bool> IsPaid { get; set; }

        public Nullable<int> TaxStartDay { get; set; }

        public Nullable<int> TaxEndDay { get; set; }

        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> OwnershipList { get; set; }
        public List<SelectListItem> LicenseList { get; set; }
        public List<SelectListItem> CurrencyList { get; set; }
        public List<SelectListItem> IndustryList { get; set; }

        public string CommanError { get; set; }

        public string TaxStartMonth { get; set; }
        public string TaxEndMonth { get; set; }
        public string Province { get; set; }

    }

    /// <summary>
    /// Active user list
    /// </summary>
    public class ActiveUserListViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string CorporationAddress { get; set; }
        public string Company { get; set; }
        public string TaxStartDay { get; set; }
        public string TaxStartYear { get; set; }
        public string TaxEndYear { get; set; }
        public string UserStatus { get; set; }
        public string EmailSendStatus { get; set; }
        public string SendMail { get; set; }
    }

    /// <summary>
    /// Terms & Condition
    /// </summary>
    public class TermsAndCondition
    {
        [Display(Name = "Terms and Conditions")]
        public bool TermAndCondition { get; set; }

        public string UserId { get; set; }
    }

    /// <summary>
    /// Create User With an accountant user
    /// </summary>
    public class UserWithAnAccountantViewModel
    {
        public int CurrencyId { get; set; }
        public int Id { get; set; }

        public int? AccountantId { get; set; }

        public int RoleId { get; set; }

        public int? SectorId { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "MobileNumber is required")]
        //[RegularExpression("([1-9][0-9]*)", ErrorMessage = "Mobile Number must be a numeric.")]
        //public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Area code is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Area code must be a numeric.")]
        public string AreaCode { get; set; }

        [Required(ErrorMessage = "MobileNumber is required")]
        [RegularExpression("([0-9][0-9]*)", ErrorMessage = "MobileNumber must be a numeric.")]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 7)]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "CountryId is required")]
        public int CountryId { get; set; }

        //   [Required(ErrorMessage = "Province is required")]
        public string Province { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "PostalCode is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Postal code must be 6 digit long.")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "CorporationAddress is required")]
        public string CorporationAddress { get; set; }

        [Required(ErrorMessage = "GSTNumber is required")]
        public string GSTNumber { get; set; }

        [Required(ErrorMessage = "BusinessNumber is required")]
        public string BusinessNumber { get; set; }

        //[Required(ErrorMessage = "TaxStartYear is required")]
        //public string TaxStartYear { get; set; }
        public List<SelectListItem> YearList { get; set; }

        //public string TaxEndYear { get; set; }

        public string PrivateKey { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsCompleted { get; set; }

        public int PageCount { get; set; }

        public int PageNumber { get; set; }

        public string Industryname { get; set; }

        // public List<Country> CountryList { get; set; }
        [Required(ErrorMessage = "Industry is required")]
        public int IndustryId { get; set; }
        public List<SelectListItem> IndustryList { get; set; }

        public bool SubIndustryError { get; set; }
        public int SubIndustryId { get; set; }
        public List<SelectListItem> SubIndustryList { get; set; }

        //  public IEnumerable<GroupedSelectListItem> CustomIndustryList { get; set; }

        [Required(ErrorMessage = "Province is required")]
        public int ProvinceId { get; set; }
        public List<SelectListItem> ProvinceList { get; set; }
        [Required(ErrorMessage = "Tax Start month is required")]
        public int TaxStartMonthId { get; set; }
        public List<SelectListItem> MonthList { get; set; }

        public int TaxEndMonthId { get; set; }
        public bool IsTrial { get; set; }
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Ownership is required")]
        public int OwnershipId { get; set; }
        public List<SelectListItem> OwnershipList { get; set; }
        public string OwnershipName { get; set; }
        [Required(ErrorMessage = "Tax Start Year is required")]
        public string TaxStartYear { get; set; }
        public string TaxStartDay { get; set; }
        public string TaxEndDay { get; set; }
        public string TaxEndYear { get; set; }
        public string TaxStartMonth { get; set; }
        public string TaxEndMonth { get; set; }
        public string SubIndustryName { get; set; }
        public int TaxationStartDay { get; set; }
        public int TaxationEndDay { get; set; }
    }

    public class UploadStatementParent : UploadStatementChild
    {

        public int selectedUserId { get; set; }
        public List<SelectListItem> UserList { get; set; }
        public int RoleId { get; set; }

        public int selectedBankId { get; set; }
        public List<SelectListItem> BankList { get; set; }
        public int UserId { get; set; }
        public List<string> friendlynameList { get; set; }
        public bool IsExisting { get; set; }

        public Nullable<int> StatementTypeId { get; set; }
    }
    public class UploadStatementChild
    {
        public string FriendlyAccountName { get; set; }
    }

    public class ReportUserForAccountant
    {
        // [Required(ErrorMessage = "Please select Accountant to link your accountant")]
        public int UserId { get; set; }
        public List<SelectListItem> UserList { get; set; }
        public int RoleId { get; set; }

    }

    public class ReconcillationViewModel
    {
        public bool enablePrintBtn { get; set; }
        public int JVID { get; set; }
        public bool IsSecondLoad { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        //UserList
        public int selectedUserId { get; set; }
        public List<SelectListItem> UserList { get; set; }
        //Classification List
        public int selectedClassificationId { get; set; }
        public List<SelectListItem> ClassificationList { get; set; }
        //Years List
        public int selectedYearId { get; set; }
        public List<SelectListItem> YearList { get; set; }
        //Month List
        public int selectedMonthId { get; set; }
        public List<SelectListItem> MonthList { get; set; }
        //Bank Type List
        public string selectedStatementTypeId { get; set; }
        public List<SelectListItem> StatementTypeList { get; set; }

        public int selectedCompanyId { get; set; }
        public List<SelectListItem> CompanyList { get; set; }

        public int selectedCategoryId { get; set; }
        public List<SelectListItem> CategoryList { get; set; }

        // public List<ReconcillationBankExpenseDto> ObjBankExpensesList { get; set; }
    }

    /// <summary>
    /// Statment List Contracts
    /// </summary>
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
        public List<SelectListItem> ExpenseList { get; set; }
        public int ExpenseId { get; set; }
        public string Status { get; set; }
        public string TransactionType { get; set; }
        public string Purpose { get; set; }
        public string Category { get; set; }
        public string Vendor { get; set; }
        //  GroupedSelectListItem
        // public IEnumerable<GroupedSelectListItem> CustomClassificationList { get; set; }
        public List<SelectListItem> ClassificationList { get; set; }
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

    public class AddCashExpenseViewModel
    {
        public string FiscalYear { get; set; }
        public bool IsVirtualEntry { get; set; }
        public int ReferenceId { get; set; }

        [Required]
        public int BankId { get; set; }
        public List<SelectListItem> BankListTypeList { get; set; }

        public string ImageName { get; set; }

        public string CloudImageName { get; set; }

        public bool IsCloud { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }
        public string CustomError { get; set; }
        public int Id { get; set; }

        public int UserId { get; set; }

        public string CashBillDate { get; set; }

        [Required(ErrorMessage = "Description is required")]

        public string Description { get; set; }
        [Required(ErrorMessage = "Comment is required")]

        public string Comment { get; set; }
        [Required(ErrorMessage = "Date is required")]
        public string Date { get; set; }
        // [Required(ErrorMessage = "Credit is required")]

        public Nullable<decimal> Credit { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<int> ClassificationId { get; set; }
        public string Classification { get; set; }
        public string Expense { get; set; }
        public List<SelectListItem> ExpenseList { get; set; }
        public int ExpenseId { get; set; }
        public string Status { get; set; }
        public string TransactionType { get; set; }
        public string Purpose { get; set; }
        public string Category { get; set; }
        public string Vendor { get; set; }
        public string customDate { get; set; }
        [Required(ErrorMessage = "Category is required")]

        public int CategoryId { get; set; }
        public List<SelectListItem> CategoryList { get; set; }

        public string BillPath { get; set; }

        public Nullable<decimal> BillTax { get; set; }

        public Nullable<decimal> BillTotal { get; set; }

        public Nullable<decimal> GSTtax { get; set; }

        public Nullable<decimal> QSTtax { get; set; }

        public Nullable<decimal> HSTtax { get; set; }

        public Nullable<decimal> PSTtax { get; set; }

        public IEnumerable<GroupedSelectListItem> CustomClassificationList { get; set; }
        public List<string> objManualEntryBills { get; set; }
        public string ImagePath { get; set; }
    }


    public class KippinStoreViewModel
    {
        public List<FolderListViewModel> ObjUserList { get; set; }
    }
    public class FolderListViewModel
    {
        public string UserName { get; set; }
        public int UserId { get; set; }
        public bool IsAssociated { get; set; }
    }

    public class FolderViewModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int UserId { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public bool IsAssociated { get; set; }
    }

    public class AddClassificationViewModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string ClassificationTypeName { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public List<SelectListItem> CategoryList { get; set; }

        [Required(ErrorMessage = "Classification Type  is required")]
        [Display(Name = "Classification Type")]
        public string ClassificationType { get; set; }

        public int ClassificationTypeId { get; set; }

        public List<SelectListItem> ClassificationTypeList { get; set; }

        public int UserID { get; set; }

        public string Type { get; set; }

        [Required(ErrorMessage = "Chart Account Number  is required")]
        [Display(Name = "Chart Account Number")]
        [StringLength(4, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string ChartAccountNumber { get; set; }

        [Display(Name = "New Chart Account Number")]
        public string NewChartAccountNumber { get; set; }

        [Required(ErrorMessage = "Classification Description  is required")]
        [Display(Name = "Classification Description")]
        public string ClassificationDesc { get; set; }

        [Required(ErrorMessage = "S type Category is required")]
        [Display(Name = "S Category")]
        public int SId { get; set; }
        public int SCategoryId { get; set; }
        public int SValues { get; set; }
        public List<SelectListItem> SName { get; set; }

        [Required(ErrorMessage = "Category2 is required")]
        [Display(Name = "Category2")]
        public int CategoryId2 { get; set; }
        public List<SelectListItem> CategoryList2 { get; set; }
        [Required(ErrorMessage = "T1 type Category is required")]
        [Display(Name = "T1 Category")]
        public int T1Id { get; set; }
        public int T1CategoryId { get; set; }
        public int T1Values { get; set; }
        public List<SelectListItem> T1Name { get; set; }

        [Required(ErrorMessage = "Category3 is required")]
        [Display(Name = "Category3")]
        public int CategoryId3 { get; set; }
        public List<SelectListItem> CategoryList3 { get; set; }
        [Required(ErrorMessage = "T2 type Category is required")]
        [Display(Name = "T2 Category")]
        public int T2Id { get; set; }
        public int T2CategoryId { get; set; }
        public int T2Values { get; set; }
        public List<SelectListItem> T2Name { get; set; }

        [Required(ErrorMessage = "Category4 is required")]
        [Display(Name = "Category4")]
        public int CategoryId4 { get; set; }
        public List<SelectListItem> CategoryList4 { get; set; }
        [Required(ErrorMessage = "Z type Category is required")]
        [Display(Name = "Z Category")]
        public int ZId { get; set; }
        public int ZCategoryId { get; set; }
        public int ZValues { get; set; }
        public List<SelectListItem> ZName { get; set; }

        public String prefixlbl { get; set; }
        public String Name { get; set; }

        [Display(Name = "Range of Act")]
        public string RangeofAct { get; set; }

        public string CategoryValue { get; set; }

    }
    public class OldBankData
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public Nullable<decimal> Debit { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> BillTax { get; set; }
        public Nullable<decimal> BillTotal { get; set; }
        public string AccountType { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string AccountNumber { get; set; }
        public string Expense { get; set; }
        public string Status { get; set; }
        public string TransactionType { get; set; }
        public string Purpose { get; set; }
        public string Vendor { get; set; }
        public string Category { get; set; }
        public string Comments { get; set; }
        public string AccountName { get; set; }
        public string CompanyName { get; set; }
        public string Classification { get; set; }
        public string AccountantEmail { get; set; }
    }
    public class OpeningBalanceCalculationDto
    {
        public Nullable<int> AccountClassificationId { get; set; }
        public Nullable<int> ClassificationId { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal CreditOpeningBalance { get; set; }
        public decimal DebitOpeningBalance { get; set; }
        public bool isDebit { get; set; }
        public Nullable<decimal> GSTtax { get; set; }
        public Nullable<decimal> QSTtax { get; set; }
        public Nullable<decimal> HSTtax { get; set; }
        public Nullable<decimal> PSTtax { get; set; }
        public Nullable<double> BusinessPercentage { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string AccountName { get; set; }
        public int UserId { get; set; }
        public string UploadType { get; set; }
        public Nullable<int> BankId { get; set; }
    }
}