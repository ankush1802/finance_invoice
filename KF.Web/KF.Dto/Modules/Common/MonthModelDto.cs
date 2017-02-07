using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Common
{
    [DataContract]
    public class MonthModelDto : ApiResponseDto
    {
        [DataMember]
        public byte Id { get; set; }
        [DataMember]
        public string Month { get; set; }
    }
}
