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
    public class InvoiceUserRegistrationDto : ApiResponseDto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string CompanyLogo { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string ContactPerson { get; set; }
        [DataMember]
        public string CorporateAptNo { get; set; }
        [DataMember]
        public string CorporateHouseNo { get; set; }
        [DataMember]
        public string CorporateStreet { get; set; }
        [DataMember]
        public string CorporateCity { get; set; }
        [DataMember]
        public string CorporateState { get; set; }
        [DataMember]
        public string CorporatePostalCode { get; set; }
        [DataMember]
        public string ShippingAptNo { get; set; }
        [DataMember]
        public string ShippingHouseNo { get; set; }
        [DataMember]
        public string ShippingStreet { get; set; }
        [DataMember]
        public string ShippingCity { get; set; }
        [DataMember]
        public string ShippingState { get; set; }
        [DataMember]
        public string ShippingPostalCode { get; set; }
        [DataMember]
        public string GoodsType { get; set; }
        [DataMember]
        public string BusinessNumber { get; set; }
        [DataMember]
        public string TradingCurrency { get; set; }
        [DataMember]
        public string MobileNumber { get; set; }
        [DataMember]
        public string EmailTo { get; set; }
        [DataMember]
        public string EmailCc { get; set; }
        [DataMember]
        public string Website { get; set; }
        [DataMember]
        public Nullable<bool> IsDeleted { get; set; }
        [DataMember]
        public Nullable<bool> IsActive { get; set; }
        [DataMember]
        public Nullable<bool> IsOnlyInvoice { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CreatedOn { get; set; }
         [DataMember]
        public string Password { get; set; }
         [DataMember]
         public bool FromInvoiceOrFinance { get; set; }
        [DataMember]
         public string Username { get; set; }
        [DataMember]
        public int CustomerId { get; set; }
    }
    [DataContract]
    public class ChangePassword : ApiResponseDto
    {
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string OldPassword { get; set; }
        [DataMember]
        public string NewPassword { get; set; }
    }
}
