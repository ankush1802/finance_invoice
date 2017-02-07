using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
    [DataContract]
    public class ContentServiceInfo
    {
        [DataMember]
        public int contentServiceId { get; set; }

        [DataMember]
        public string contentServiceDisplayName { get; set; }

        [DataMember]
        public string organizationDisplayName { get; set; }

        [DataMember]
        public string mfaCoverage { get; set; }
        
        [DataMember]
        public MfaType mfaType { get; set; }

        [DataMember]
        public ContainerInfo containerInfo { get; set; }

    }


    [DataContract]
    public class ContainerInfo
    {
        [DataMember]
        public string containerName { get; set; }
    }


    [DataContract]
    public class MfaType
    {
        [DataMember]
        public int typeId { get; set; }
        [DataMember]
        public string typeName { get; set; }
    }
}