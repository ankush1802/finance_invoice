using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.ModelDto.DataTransferObjects
{
    public class AddCardDetailDto
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public int CardType { get; set; }
        public int CVV { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        #region Billing Address
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        #endregion


        public decimal Price { get; set; }
        public string LicenseType { get; set; }
        public int LicenseId { get; set; }
        public string Token { get; set; }
    }
}
