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
    
    public partial class tblCustomerOrSupplier
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
        public string Telephone { get; set; }
        public string Debtors { get; set; }
        public string Credetor { get; set; }
        public Nullable<bool> IsFinance { get; set; }
        public Nullable<bool> IsEmailExist { get; set; }
    }
}