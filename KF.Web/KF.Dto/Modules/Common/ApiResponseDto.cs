using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Common
{
    [Serializable()]
    [DataContract]
    public class ApiResponseDto
    {
        [DataMember]
        public int ResponseCode { get; set; }

        [DataMember]
        public string ResponseMessage { get; set; }

        //New Field Added by Ankush on 16th jan 2016
        [DataMember]
        public int UserId { get; set; }
    }
}
