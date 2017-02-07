using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
    [DataContract]
    public class MFARefreshInfo
    {
        [DataMember]
        public bool isMessageAvailable { get; set; }
        [DataMember]
        public FieldInfo fieldInfo { get; set; }
        [DataMember]
        public int timeOutTime { get; set; }
        [DataMember]
        public int itemId { get; set; }
        [DataMember]
        public int memSiteAccId { get; set; }
        [DataMember]
        public bool retry { get; set; }
        [DataMember]
        public string errorCode { get; set; }
    }

    [DataContract]
    public class FieldInfo
    {
        [DataMember]
        public string responseFieldType { get; set; }
        [DataMember]
        public string minimumLength { get; set; }
        [DataMember]
        public int maximumLength { get; set; }
        [DataMember]
        public string displayString { get; set; }
        [DataMember]
        public IList<QuestionAndAnswerValues> questionAndAnswerValues { get; set; }
        [DataMember]
        public int numOfMandatoryQuestions { get; set; }
        [DataMember]
        public string imageFieldType { get; set; }
        [DataMember]
        public sbyte[] image { get; set; }
    }

    [DataContract]
    public class QuestionAndAnswerValues
    {
        [DataMember]
        public string question { get; set; }
        [DataMember]
        public string questionFieldType { get; set; }
        [DataMember]
        public string responseFieldType { get; set; }
        [DataMember]
        public bool isRequired { get; set; }
        [DataMember]
        public int sequence { get; set; }
        [DataMember]
        public string metaData { get; set; }
    }
}
