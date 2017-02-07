using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using YodleeBase.model;

namespace YodleeBase.model
{
    [DataContract]
    public class UserContext
    {
        [DataMember]
        public ConversationCredentials conversationCredentials { get; set; }

        [DataMember]
        public string cobrandId { get; set; }
    }
}
