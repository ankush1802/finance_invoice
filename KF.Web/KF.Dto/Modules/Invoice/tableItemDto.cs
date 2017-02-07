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
    public partial class ItemtableDto : ApiResponseDto
    {
         [DataMember]
        public int Id { get; set; }
         [DataMember]
        public Nullable<int> ItemId { get; set; }
         [DataMember]
        public string ServiceType { get; set; }
         [DataMember]
        public string Description { get; set; }
         [DataMember]
     
        public Nullable<int> Quantity { get; set; }
         [DataMember]
        public Nullable<decimal> Rate { get; set; }
         [DataMember]
        public Nullable<decimal> Amount { get; set; }
         [DataMember]
        public string Item { get; set; }
         [DataMember]
        public Nullable<int> InvoiceId { get; set; }
         [DataMember]

        public string Discount { get; set; }
         [DataMember]
        public Nullable<int> ServiceTypeId { get; set; }
           [DataMember]
         public string Tax { get; set; }
           [DataMember]
         public string PST_Tax { get; set; }
           [DataMember]
         public string HST_Tax { get; set; }
           [DataMember]
         public string QST_Tax { get; set; }
           [DataMember]
         public string GST_Tax { get; set; }
           [DataMember]
           public Nullable<decimal> SubTotal { get; set; }
    }
}
