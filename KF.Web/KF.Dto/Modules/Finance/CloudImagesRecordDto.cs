using KF.Dto.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Finance
{
     [DataContract]
    public class CloudImagesRecordDto : ApiResponseDto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string ImageName { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DateCreated { get; set; }
        [DataMember]
        public Nullable<bool> IsAssociated { get; set; }
        [DataMember]
        public Nullable<int> StatementId { get; set; }
        [DataMember]
        public Nullable<int> UserId { get; set; }
        [DataMember]
        public Nullable<int> Year { get; set; }
        [DataMember]
        public Nullable<int> Month { get; set; }

    }
}
