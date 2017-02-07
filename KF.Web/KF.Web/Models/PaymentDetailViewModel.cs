using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KF.Web.Models
{
    public class PaymentDetailViewModel
    {
        public int UserId { get; set; }
        public string CardNumber { get; set; }
        public int CardType { get; set; }
        public int CVV { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        [Required(ErrorMessage = "Please insert your name")]
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