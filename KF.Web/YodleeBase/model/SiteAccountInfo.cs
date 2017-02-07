using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
	[DataContract]
	public class SiteAccountInfo
	{
		[DataMember]
		public int siteAccountId { get; set; }

        [DataMember]
        public bool isCustom { get; set; }

        [DataMember]
        public int credentialsChangedTime { get; set; }

        [DataMember]
        public SiteRefreshInfo siteRefreshInfo { get; set; }

        [DataMember]
        public SiteInfo siteInfo { get; set; }

        [DataMember]
        public int retryCount { get; set; }
	}
}
