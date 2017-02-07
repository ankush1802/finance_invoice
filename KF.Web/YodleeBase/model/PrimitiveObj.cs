using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
    [DataContract]
    public class PrimitiveObj
    {
        [DataMember]
        public string primitiveObj { get; set; }
    }
}
