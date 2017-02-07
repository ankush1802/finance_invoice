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
    public class CurrencyDto : ApiResponseDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string CurrencyType { get; set; }
    }
}
