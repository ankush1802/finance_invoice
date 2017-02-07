using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
    [DataContract]
    public class FormObject
    {
        [DataMember]
        public ConjunctionOp conjunctionOp { get; set; }

        [DataMember]
        public IList<ComponentList> componentList { get; set; }

        [DataMember]
        public string defaultHelpText { get; set; }
    }

    [DataContract]
    public class ConjunctionOp
    {
        [DataMember]
        public string conjuctionOp { get; set; }
    }

    [DataContract]
    public class ComponentList
    {
        [DataMember]
        public string valueIdentifier { get; set; }
        [DataMember]
        public string valueMask { get; set; }
        [DataMember]
        public FieldType fieldType { get; set; }
        [DataMember]
        public string size { get; set; }
        [DataMember]
        public string maxlength { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string displayName { get; set; }
        [DataMember]
        public string isEditable { get; set; }
        [DataMember]
        public string isOptional { get; set; }
        [DataMember]
        public string isEscaped { get; set; }
        [DataMember]
        public string isOptionalMFA { get; set; }
        [DataMember]
        public string isMFA { get; set; }
        [DataMember]
        public string helpText { get; set; }
    }

    [DataContract]
    public class FieldType
    {
        [DataMember]
        public string typeName { get; set; }
    }

}
