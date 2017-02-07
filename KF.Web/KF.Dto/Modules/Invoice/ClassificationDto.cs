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
    public partial class ClassificationDto : ApiResponseDto
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
    public partial class ClassificationTypeDto : ApiResponseDto
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string ClassificationType { get; set; }
    }

       public partial class CustomClassificationDto 
       {
           public int Id { get; set; }
           public string Category { get; set; }
           public int CategoryId { get; set; }
           public string ClassificationType { get; set; }
       }

       public partial class CustomIndustryDto
       {
           public int SubIndustryId { get; set; }
           public string Industry { get; set; }
           public Nullable<int> IndustryId { get; set; }
           public string SubIndustryType { get; set; }
       }
}
