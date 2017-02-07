using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
	[DataContract]
	public class CobrandContext
	{
		[DataMember]
        public ConversationCredentials cobrandConversationCredentials { get; set; }
	}

	[DataContract]
	public class ConversationCredentials
	{
		[DataMember]
		public string sessionToken { get; set; }
	}
}
