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
    
    public partial class KippinStoreImage
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<int> Month { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<bool> IsAssociated { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}