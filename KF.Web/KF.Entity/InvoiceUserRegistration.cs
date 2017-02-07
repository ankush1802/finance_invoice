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
    using System.Collections.Generic;
    
    public partial class InvoiceUserRegistration
    {
        public int Id { get; set; }
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
        public string BusinessNumber { get; set; }
        public string TradingCurrency { get; set; }
        public string MobileNumber { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
        public string Website { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<bool> IsTrial { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public Nullable<bool> IsVerified { get; set; }
        public Nullable<bool> IsOnlyInvoice { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
