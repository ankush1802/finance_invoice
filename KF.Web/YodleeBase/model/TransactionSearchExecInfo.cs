using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
    [DataContract]
    public class TransactionSearchExecInfo
    {
        [DataMember]
        public SearchIdentifier searchIdentifier { get; set; }

        [DataMember]
        public int numberOfHits { get; set; }

        [DataMember]
        public SearchResult searchResult { get; set; }

        [DataMember]
        public int countOfAllTransaction { get; set; }
    }

    [DataContract]
    public class SearchIdentifier
    {
        [DataMember]
        public string identifier { get; set; }
    }

    [DataContract]
    public class SearchResult
    {
        [DataMember]
        public ICollection<Transactions> transactions { get; set; }
    }
    [DataContract]
    public class ViewKey
    {
        [DataMember]
        public Int64 transactionId { get; set; }

        [DataMember]
        public String containerType { get; set; }

        [DataMember]
        public Int64 transactionCount { get; set; }

        [DataMember]
        public Int64 rowNumber { get; set; }

        [DataMember]
        public Boolean isParentMatch { get; set; }

        [DataMember]
        public Boolean isSystemGeneratedSplit { get; set; }

    }
    [DataContract]
    public class Transactions
    {
        [DataMember]
        public ViewKey viewKey { get; set; }
        [DataMember]
        public Description description { get; set; }

        [DataMember]
        public string transactionDate { get; set; }

        [DataMember]
        public string postDate { get; set; }

        [DataMember]
        public YMoney amount { get; set; }

        [DataMember]
        public Status status { get; set; }

        [DataMember]
        public string transactionBaseType { get; set; }

        [DataMember]
        public Account account { get; set; }
    }

    [DataContract]
    public class Description
    {
        [DataMember]
        public string description { get; set; }
    }

    [DataContract]
    public class Status
    {
        [DataMember]
        public string description { get; set; }
    }
}


