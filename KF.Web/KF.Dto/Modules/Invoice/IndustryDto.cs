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
    public class IndustryDto : ApiResponseDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string IndustryType { get; set; }
    }
}
