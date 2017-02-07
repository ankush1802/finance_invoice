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
    public class AddCustomerSupplierDto : ApiResponseDto
    {
         [DataMember]
        public int Id { get; set; }
         [DataMember]
        public string Company_Name { get; set; }
         [DataMember]
        public string FirstName { get; set; }
         [DataMember]
        public string Address { get; set; }
         [DataMember]
        public string City { get; set; }
         [DataMember]
        public string State { get; set; }
         [DataMember]
        public string PostalCode { get; set; }
          [DataMember]
        public string ServiceOffered { get; set; }
          [DataMember]
        public string Mobile { get; set; }
          [DataMember]
        public string Email { get; set; }
          [DataMember]
        public string Website { get; set; }
          [DataMember]
        public Nullable<int> UserId { get; set; }
          [DataMember]
        public Nullable<System.DateTime> DateCreated { get; set; }
          [DataMember]
        public Nullable<System.DateTime> DateModify { get; set; }
          [DataMember]
        public Nullable<bool> Isdeleted { get; set; }
          [DataMember]
        public Nullable<int> RoleId { get; set; }
          [DataMember]
        public string AdditionalEmail { get; set; }
          [DataMember]
        public string ShippingAddress { get; set; }
          [DataMember]
        public string ShippingCity { get; set; }
          [DataMember]
        public string ShippingState { get; set; }
          [DataMember]
        public string ShippingPostalCode { get; set; }
         [DataMember]
          public Nullable<bool> IsFinance { get; set; }
         [DataMember]
         public Nullable<bool> IsEmailExist{ get; set; }
          [DataMember]
         public string Telephone { get; set; }
         [DataMember]
          public string Debtors { get; set; }
         [DataMember]
          public string Credetor { get; set; }
    }
}
