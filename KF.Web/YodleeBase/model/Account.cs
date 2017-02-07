using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace YodleeBase.model
{
  

    [DataContract]
    public class Account
    {
        [DataMember]
        public string siteName { get; set; }
        [DataMember]
        public int ResponseCode { get; set; }
        [DataMember]
        public int siteAccountId { get; set; }
        [DataMember]
        public string BankName { get; set; }
        [DataMember]
        public string ResponseMessage { get; set; }
        [DataMember]
        public AccountDisplayName accountDisplayName { get; set; }

        [DataMember]
        public string itemAccountId { get; set; }

        [DataMember]
        public string accountName { get; set; }

        [DataMember]
        public YMoney currentBalance { get; set; }

        [DataMember]
        public YMoney accountBalance { get; set; }

        [DataMember]
        public YMoney availableBalance { get; set; }

        [DataMember]
        public YMoney totalBalance { get; set; }

        [DataMember]
        public int created { get; set; }

        [DataMember]
        public DateTime createdAsDateTime
        {
            get
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                            .AddSeconds(this.created / 1000)
                            .ToLocalTime();
            }
        }

        [DataMember]
        public int lastUpdated { get; set; }

        public DateTime lastUpdatedAsDateTime
        {
            get
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                            .AddSeconds(this.lastUpdated / 1000)
                            .ToLocalTime();
            }
        }

        [DataMember]
        public string accountNumber { get; set; }

        [DataMember]
        public string accountHolder { get; set; }

        [DataMember]
        public YMoney lastPaymentAmount { get; set; }

        [DataMember]
        public YDate lastPaymentDate { get; set; }

        #region Loans
        [DataMember]
        public ICollection<Loan> loans { get; set; }

        [DataMember]
        public string loanAccountNumber { get; set; }
        #endregion Loans

        [DataMember]
        public ICollection<Bill> bills { get; set; }

        [DataMember]
        public ICollection<Holdings> holdings { get; set; }

        #region Credit Card
        [DataMember]
        public YMoney runningBalance { get; set; }

        [DataMember]
        public YMoney amountDue { get; set; }

        [DataMember]
        public YMoney lastPayment { get; set; }
        #endregion Credit Card

        #region Rewards
        [DataMember]
        public ICollection<RewardActivity> rewardActivities { get; set; }

        [DataMember]
        public ICollection<RewardBalance> rewardsBalances { get; set; }
        #endregion Rewards

        [DataMember]
        public ICollection<InsurancePolicy> insurancePolicys { get; set; }

        #region Stocks
        [DataMember]
        public YMoney cash { get; set; }

        [DataMember]
        public string planName { get; set; }
        #endregion Stocks

    }

    #region Base
    [DataContract]
    public class AccountDisplayName
    {
        [DataMember]
        public string defaultNormalAccountName { get; set; }
    }

    [DataContract]
    public class YMoney
    {
        [DataMember]
        public decimal amount { get; set; }

        [DataMember]
        public string currencyCode { get; set; }
    }

    [DataContract]
    public class YDate
    {
        [DataMember]
        public string date { get; set; }

        [DataMember]
        public string localFormat { get; set; }
    }
    #endregion Base

    [DataContract]
    public class Holdings : Account
    {
        [DataMember]
        public string holdingType { get; set; }
        [DataMember]
        public string symbol { get; set; }
        [DataMember]
        public string cusipNumber { get; set; }
        [DataMember]
        public YMoney price { get; set; }
        [DataMember]
        public YMoney value { get; set; }
        [DataMember]
        public string description { get; set; }
    }

    [DataContract]
    public class RewardActivity : Account
    {
        [DataMember]
        public int totalUnit { get; set; }

        [DataMember]
        public YMoney totalAmount { get; set; }

        [DataMember]
        public string memberName { get; set; }
    }

    [DataContract]
    public class RewardBalance : Account
    {
        [DataMember]
        public double balanceAmount { get; set; }

        [DataMember]
        public string balanceUnit { get; set; }

        [DataMember]
        public string memberName { get; set; }
    }

    [DataContract]
    public class Loan : Account
    {
        [DataMember]
        public YMoney recurringPayment { get; set; }

        [DataMember]
        public string loanAccountNumber { get; set; }

        [DataMember]
        public string loanType { get; set; }

        [DataMember]
        public string lender { get; set; }

        [DataMember]
        public YMoney originalLoanAmount { get; set; }
    }

    [DataContract]
    public class InsurancePolicy : Account
    {
        [DataMember]
        public string policyStatus { get; set; }
    }

    [DataContract]
    public class Bill : Account
    {
        [DataMember]
        public YDate paymDate { get; set; }

        [DataMember]
        public string acctType { get; set; }

        [DataMember]
        public YMoney payment { get; set; }

        [DataMember]
        public YDate paymentDate { get; set; }

        [DataMember]
        public string paymFreqCode { get; set; }
    }

}
