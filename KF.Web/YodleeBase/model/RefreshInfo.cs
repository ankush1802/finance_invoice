using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
    [DataContract]
    public class RefreshInfo
    {
        [DataMember]
        public int itemId { get; set; }

        [DataMember]
        public int statusCode { get; set; }

        [DataMember]
        public int refreshType { get; set; }

        [DataMember]
        public int refreshRequestTime { get; set; }

        [DataMember]
        public int lastUpdatedTime { get; set; }

        [DataMember]
        public int lastUpdateAttemptTime { get; set; }
        
        [DataMember]
        public GenericDescription itemAccessStatus { get; set; }

        [DataMember]
        public GenericDescription userActionRequiredType { get; set; }
  
        [DataMember]
        public int userActionRequiredCode { get; set; }

        [DataMember]
        public string userActionRequiredSince { get; set; }
   
        [DataMember]
        public GenericDate lastDataUpdateAttempt { get; set; }
 
        [DataMember]
        public GenericDate lastUserRequestedDataUpdateAttempt { get; set; }

        [DataMember]
        public string lastSuccessfulDataUpdate { get; set; }

        [DataMember]
        public string itemCreateDate { get; set; }

        [DataMember]
        public int nextUpdateTime { get; set; }

        [DataMember]
        public ResponseCodeType responseCodeType { get; set; }

        [DataMember]
        public int retryCount { get; set; }

        [DataMember]
        public string refreshMode { get; set; }
    }

    [DataContract]
    public class ResponseCodeType {
        [DataMember]
        public int responseCodeTypeId { get; set; }
    }

    [DataContract]
    public class GenericDate 
    {
        [DataMember]
        public string date { get; set; }
        
        [DataMember]
        public GenericDescription status { get; set; }

        [DataMember]
        public GenericDescription type { get; set; }
    }



    [DataContract]
    public class GenericDescription
    {
        [DataMember]
        public string name { get; set; }
    }
}
