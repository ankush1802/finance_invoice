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
    public class ClassificationDto : ApiResponseDto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public string ClassificationType { get; set; }
        [DataMember]
        public string Desc { get; set; }
        [DataMember]
        public string Type { get; set; }
    }
    [DataContract]
    public class ClassificationTypeDto : ApiResponseDto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string ClassificationType { get; set; }
    }
}
