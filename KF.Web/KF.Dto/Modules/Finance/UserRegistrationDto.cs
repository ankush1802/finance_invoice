using KF.Dto.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Finance
{
    [Serializable()]
    [DataContract]
    public class UserRegistrationDto : ApiResponseDto
    {
        //[DataMember]
        //public string Country { get; set; }
        //[DataMember]
        //public int Id { get; set; }
        //[DataMember]
        //public Nullable<int> AccountantId { get; set; }
        //[DataMember]
        //public string FirstName { get; set; }
        //[DataMember]
        //public string LastName { get; set; }
        //[DataMember]
        //public string Username { get; set; }
        //[DataMember]
        //public string Email { get; set; }
        //[DataMember]
        //public string Password { get; set; }
        //[DataMember]
        //public string MobileNumber { get; set; }
        //[DataMember]
        //public Nullable<int> CountryId { get; set; }
        //[DataMember]
        //public Nullable<byte> ProvinceId { get; set; }
        //[DataMember]
        //public string City { get; set; }
        //[DataMember]
        //public string PostalCode { get; set; }
        //[DataMember]
        //public Nullable<int> SectorId { get; set; }
        //[DataMember]
        //public Nullable<int> IndustryId { get; set; }
        //[DataMember]
        //public string CompanyName { get; set; }
        //[DataMember]
        //public string CorporationAddress { get; set; }
        //[DataMember]
        //public string GSTNumber { get; set; }
        //[DataMember]
        //public string BusinessNumber { get; set; }
        //[DataMember]
        //public Nullable<int> OwnershipId { get; set; }
        //[DataMember]
        //public Nullable<int> CurrencyId { get; set; }
        //[DataMember]
        //public Nullable<int> LicenseId { get; set; }
        //[DataMember]
        //public Nullable<int> RoleId { get; set; }
        //[DataMember]
        //public Nullable<int> TaxStartYear { get; set; }
        //[DataMember]
        //public Nullable<int> TaxEndYear { get; set; }
        //[DataMember]
        //public string PrivateKey { get; set; }
        //[DataMember]
        //public Nullable<System.DateTime> CreatedDate { get; set; }
        //[DataMember]
        //public Nullable<System.DateTime> ModifiedDate { get; set; }
        //[DataMember]
        //public Nullable<bool> IsCompleted { get; set; }
        //[DataMember]
        //public Nullable<byte> TaxStartMonthId { get; set; }
        //[DataMember]
        //public Nullable<byte> TaxEndMonthId { get; set; }
        //[DataMember]
        //public Nullable<bool> IsTrial { get; set; }
        //[DataMember]
        //public Nullable<bool> IsEmailSent { get; set; }
        //[DataMember]
        //public string Status { get; set; }
        //[DataMember]
        //public Nullable<bool> IsVerified { get; set; }
        //[DataMember]
        //public Nullable<bool> IsPaid { get; set; }
        //[DataMember]
        //public Nullable<int> TaxationStartDay { get; set; }
        //[DataMember]
        //public Nullable<int> TaxationEndDay { get; set; }
        //[DataMember]
        //public Nullable<bool> IsDeleted { get; set; }

        //[DataMember]
        //public Nullable<bool> IsUnlinkUser { get; set; }
        //[DataMember]
        //public bool IsDownload { get; set; }
        //[DataMember]
        //public Nullable<bool> IsUnlink { get; set; }
        //[DataMember]
        //public Nullable<int> SubIndustryId { get; set; }
        //[DataMember]
        //public string ProfileImage { get; set; }

        //public Nullable<bool> IsEmployeeActivated { get; set; }

        //[DataMember]
        //public Nullable<bool> IsOnlyInvoice { get; set; }
        
        [DataMember]
        public string Country1 { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public Nullable<int> AccountantId { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string MobileNumber { get; set; }
        [DataMember]
        public Nullable<int> CountryId { get; set; }
        [DataMember]
        public Nullable<byte> ProvinceId { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string PostalCode { get; set; }
        [DataMember]
        public Nullable<int> SectorId { get; set; }
        [DataMember]
        public Nullable<int> IndustryId { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string CorporationAddress { get; set; }
        [DataMember]
        public string GSTNumber { get; set; }
        [DataMember]
        public string BusinessNumber { get; set; }
        [DataMember]
        public Nullable<int> OwnershipId { get; set; }
        [DataMember]
        public Nullable<int> CurrencyId { get; set; }
        [DataMember]
        public Nullable<int> LicenseId { get; set; }
        [DataMember]
        public Nullable<int> RoleId { get; set; }
        [DataMember]
        public Nullable<int> TaxStartYear { get; set; }
        [DataMember]
        public Nullable<int> TaxEndYear { get; set; }
        [DataMember]
        public string PrivateKey { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DataMember]
        public Nullable<bool> IsCompleted { get; set; }
        [DataMember]
        public Nullable<byte> TaxStartMonthId { get; set; }
        [DataMember]
        public Nullable<byte> TaxEndMonthId { get; set; }
        [DataMember]
        public Nullable<bool> IsTrial { get; set; }
        [DataMember]
        public Nullable<bool> IsEmailSent { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public Nullable<bool> IsVerified { get; set; }
        [DataMember]
        public Nullable<bool> IsPaid { get; set; }
        [DataMember]
        public Nullable<int> TaxationStartDay { get; set; }
        [DataMember]
        public Nullable<int> TaxationEndDay { get; set; }
        [DataMember]
        public Nullable<bool> IsDeleted { get; set; }
        [DataMember]
        public Nullable<bool> IsUnlinkUser { get; set; }
        [DataMember]
        public bool IsDownload { get; set; }
        [DataMember]
        public Nullable<bool> IsUnlink { get; set; }
        [DataMember]
        public Nullable<int> SubIndustryId { get; set; }
        [DataMember]
        public string ProfileImage { get; set; }

        public Nullable<bool> IsEmployeeActivated { get; set; }

        [DataMember]
        public Nullable<bool> IsOnlyInvoice { get; set; }


        [DataMember]
        public int InvoiceId { get; set; }
        [DataMember]
        public string CompanyLogo { get; set; }

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
        public int CustomerId { get; set; }

    }
}
