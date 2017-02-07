using KF.Dto.Modules.Common;
using KF.Entity;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Repo.Modules.Common
{
    public class PaymentRepository : IDisposable
    {
        #region Dispose
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
        /// <summary>
        /// Get Stripe Secret Key
        /// </summary>
        public static string stripesecretkey = ConfigurationSettings.AppSettings["SecretKey"];

        /// <summary>
        /// Stripe Payment
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public string ProcessUserActivationPayment(string tokenId, decimal price)
        {

            try
            {
                var myCharge = new StripeChargeCreateOptions
                {
                    // convert the amount of £12.50 to pennies i.e. 1250
                    Amount = (int)price * 100,
                    Currency = "USD",
                    Description = "Description for test charge",
                    Source = new StripeSourceOptions
                    {
                        TokenId = tokenId,
                    }
                };

                var chargeService = new StripeChargeService(stripesecretkey);
                var stripeCharge = chargeService.Create(myCharge);
                return stripeCharge.Id;
            }
            catch (Exception Ex)
            {
                var exception = Ex.InnerException;
                return exception.Message;
            }

        }


        #region Insert Card Details
        public bool AddCard(AddCardDetailDto Obj)
        {
            bool IsSaved = false;
            using (var db = new KFentities())
            {
                var existChk = db.CardDetails.Where(p => p.CardNumber == Obj.CardNumber && p.UserId == Obj.UserId).FirstOrDefault();
                if (existChk != null)
                {
                    //update
                    IsSaved = true;
                }
                else
                {
                    //insert
                    CardDetail dbInsert = new CardDetail();
                    dbInsert.CardNumber = Obj.CardNumber;
                    dbInsert.CardHolderName = Obj.FirstName + Obj.LastName;
                    dbInsert.CVV = Convert.ToInt32(Obj.CVV);
                    dbInsert.DateCreated = DateTime.Now;
                    dbInsert.ExpiryMonth = Obj.ExpiryMonth;
                    dbInsert.ExpiryYear = Obj.ExpiryYear;
                    dbInsert.IsDeleted = false;
                    dbInsert.UserId = Obj.UserId;
                    db.CardDetails.Add(dbInsert);
                    // db.SaveChanges();
                    IsSaved = true;

                }
            }
            return IsSaved;
        }
        #endregion

        #region Stripe Subscription
        public Boolean CreateCustomerSubscription(String Email, String username, String CardNumber, String ExpirationMonth, String ExpirationYear, String Cvc, DateTime subscriptionDate)
        {
            var myCustomer = new StripeCustomerCreateOptions();
            myCustomer.Email = Email;
            myCustomer.Description = username + " (" + Email + ")";

            StripeCreditCardOptions card = new StripeCreditCardOptions();
            card.Number = CardNumber;
            card.ExpirationMonth = ExpirationMonth;
            card.ExpirationYear = ExpirationYear;
            card.Cvc = Cvc;

            myCustomer.TaxPercent = Convert.ToDecimal(ConfigurationSettings.AppSettings["TaxPercent"]);
            myCustomer.PlanId = ConfigurationSettings.AppSettings["PlanId"];
            myCustomer.TrialEnd = subscriptionDate; // DateTime.Now.AddMonths(1); 
            var customerService = new StripeCustomerService(stripesecretkey);

            try
            {
                StripeCustomer result = customerService.Create(myCustomer);
                return true;
            }
            catch (StripeException)
            {
                throw;
            }

        }
        #endregion
    }
}

