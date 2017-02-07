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
    public class IndustryListDto : ApiResponseDto
    {
        [DataMember]
        public int IndustryId { get; set; }
        [DataMember]
        public string IndustryName { get; set; }
        [DataMember]
        public List<SubIndustryList> ObjSubIndustryList { get; set; }

    }
    [DataContract]
    public class SubIndustryList
    {
        [DataMember]
        public int SubIndustryId { get; set; }
        [DataMember]
        public string SubIndustryName { get; set; }
    }
}
