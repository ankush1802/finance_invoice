using System;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
	[DataContract]
	public class SiteInfo
	{
		[DataMember]
		public int siteId { get; set; }

		[DataMember]
		public string defaultDisplayName { get; set; }


        [DataMember]
        public string defaultOrgDisplayName { get; set; }
        
	}
}

