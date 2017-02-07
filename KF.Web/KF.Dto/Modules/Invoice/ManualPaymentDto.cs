using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Invoice
{
    [DataContract]
    public class ManualPaymentDto
    {
        [DataMember]
        public Nullable<bool> IsSupplierManualPaid { get; set; }
        [DataMember]
        public Nullable<decimal> SupplierManualPaidAmount { get; set; }
        [DataMember]
        public string SupplierManualPaidJVID { get; set; }
        [DataMember]
        public Nullable<bool> IsCustomerManualPaid { get; set; }
        [DataMember]
        public Nullable<decimal> CustomerManualPaidAmount { get; set; }
        [DataMember]
        public string CustomerManualPaidJVID { get; set; }
        [DataMember]
        public int InvoiceId { get; set; }
        [DataMember]
        public bool IsCustomer { get; set; }
    }
}
