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
    
    public partial class YodleeAccountDetail
    {
        public int Id { get; set; }
        public long ItemAccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountUserName { get; set; }
        public string AccountPassword { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string CobrandToken { get; set; }
        public string UserToken { get; set; }
        public Nullable<int> UserId { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public Nullable<int> BankId { get; set; }
        public string AccountType { get; set; }
    }
}