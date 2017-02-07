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
    public class StripePaymentDto : ApiResponseDto
    {
        [DataMember]
        public int InvoiceId { get; set; }

        [DataMember]
        public decimal PaidAmount { get; set; }

        [DataMember]
        public string PaidJVID { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public int ExpiryMonth { get; set; }

        [DataMember]
        public int Cvcs { get; set; }

        [DataMember]
        public int ExpiryYear { get; set; }

        [DataMember]
        public int SupplierID { get; set; }

    }
}
