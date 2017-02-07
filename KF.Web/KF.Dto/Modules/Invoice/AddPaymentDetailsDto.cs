using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.ModelDto.DataTransferObjects
{
    [DataContract]
    public class AddPaymentDetailsDto 
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Name is required")]
        public string CardHolderName { get; set; }
        [DataMember]
        public Nullable<int> ExpiryMonth { get; set; }
        [DataMember]
        public Nullable<int> ExpiryYear { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DateModified { get; set; }
        [DataMember]
        [Required(ErrorMessage = "CVV Number is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "CVV Number must be a numeric.")]
        public Nullable<int> CVV { get; set; }
        [DataMember]
        public Nullable<bool> IsDeleted { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Card Number is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Card Number must be a numeric.")]
        public string CardNumber { get; set; }
        [DataMember]
        public Nullable<int> UserId { get; set; }
    }

    public class AddStripeDetailsDto 
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Publishable Key is required")]
        public string PublishableKey { get; set; }
        [Required(ErrorMessage = "Secret Key is required")]
        public string SecretKey { get; set; }       
        public string Email { get; set; }
    }
}
