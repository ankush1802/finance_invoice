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
    public partial class Kl_BlockListDto : ApiResponseDto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public string ClassificationType { get; set; }
        [DataMember]
        public string ChartAccountNumber { get; set; }
        [DataMember]
        public Nullable<System.DateTime> CreatedDate { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        [DataMember]
        public Nullable<bool> IsDeleted { get; set; }
        [DataMember]
        public Nullable<bool> IsSole { get; set; }
        [DataMember]
        public Nullable<bool> IsIncorporated { get; set; }
        [DataMember]
        public Nullable<bool> IsPartnerShip { get; set; }
        [DataMember]
        public string Desc { get; set; }
        [DataMember]
        public Nullable<int> UserId { get; set; }
        [DataMember]
        public Nullable<int> IndustryId { get; set; }
        [DataMember]
        public Nullable<int> SubIndustryId { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string RangeofAct { get; set; }
        [DataMember]
        public string CategoryValue { get; set; }

    }
}
