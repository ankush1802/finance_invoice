using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KF.Dto.Modules.Common
{
    [DataContract]
    public class LoginWebsiteDto
    {
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public int BankId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string AddAccountUserName { get; set; }
        [DataMember]
        public string AddAccountPassword { get; set; }

        public string CobrandToken { get; set; }

        public string UserToken { get; set; }

        public string Error { get; set; }
    }
}
