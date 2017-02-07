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
    public class InvoiceCurrencyDto : ApiResponseDto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string CurrencyName { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [DataMember]
        public Nullable<bool> IsDeleted { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string Symbol { get; set; }
        [DataMember]
        public string SymbolCode { get; set; }
    }
}
