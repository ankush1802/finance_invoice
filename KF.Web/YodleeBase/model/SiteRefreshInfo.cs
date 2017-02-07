using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
    [DataContract]
    public class SiteRefreshInfo
    {
        [DataMember]
        public SiteRefreshStatus siteRefreshStatus { get; set; }

        [DataMember]
        public SiteRefreshMode siteRefreshMode { get; set; }

        [DataMember]
        public int updateInitTime { get; set; }

        [DataMember]
        public int nextUpdate { get; set; }

        [DataMember]
        public int code { get; set; }

        [DataMember]
        public SuggestedFlow suggestedFlow { get; set; }

        public int noOfRetry { get; set; }
    }

    [DataContract]
    public class SiteRefreshStatus
    {
        [DataMember]
        public string siteRefreshStatusId { get; set; }

        [DataMember]
        public string siteRefreshStatus { get; set; }
    }

    [DataContract]
    public class SiteRefreshMode
    {
        [DataMember]
        public string refreshModeId { get; set; }

        [DataMember]
        public string refreshMode { get; set; }
    }

    [DataContract]
    public class SuggestedFlow
    {
        [DataMember]
        public int statsuggestedFlowId { get; set; }
        [DataMember]
        public string suggestedFlow { get; set; }
    }
}
