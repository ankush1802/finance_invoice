using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Common
{
    [DataContract]
    public class SendMailModelDto
    {
        [DataMember]
        public string From { get; set; }
        [DataMember]
        public string To { get; set; }
        [DataMember]
        public string MessageBody { get; set; }
        [DataMember]
        public string Subject { get; set; }
    }
}
