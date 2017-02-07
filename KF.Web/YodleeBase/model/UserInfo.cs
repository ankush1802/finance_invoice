using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public UserContext userContext { get; set; }

        [DataMember]
        public string loginName { get; set; }
    }
}
