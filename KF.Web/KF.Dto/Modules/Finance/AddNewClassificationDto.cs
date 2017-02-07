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
    public class AddNewClassificationDto : ApiResponseDto
    {
        [DataMember]
        public int ClassificationTypeId { get; set; }
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public string ctext { get; set; }
        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public string cDesc { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string ChartAccountNumber { get; set; }
    }
    public class CustomClassificationDto
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string ClassificationType { get; set; }
    }
}
