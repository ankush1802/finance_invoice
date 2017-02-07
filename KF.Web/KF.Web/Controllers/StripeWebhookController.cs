using Stripe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace KF.Web.Controllers
{
    public class StripeWebhookController : Controller
    {
        // GET: StripeWebhook
        [HttpPost]
        public HttpStatusCodeResult StripeResponse()
        {
            string response = string.Empty;

            // MVC3/4: Since Content-Type is application/json in HTTP POST from Stripe
            // we need to pull POST body from request stream directly
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);

            string json = new StreamReader(req).ReadToEnd();
            StripeEvent stripeEvent = null;
            try
            {
                // it's a great library that should have been offered by Stripe directly
                stripeEvent = StripeEventUtility.ParseEvent(json);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Unable to parse incoming event");
            }

            if (stripeEvent == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Incoming event empty");

            switch (stripeEvent.Type)
            {
                case "charge.succeeded":
                    // do work
                    string stripeCustomerId = stripeEvent.UserId;
                    string status = stripeEvent.Data.Object.status;
                    if (status.Equals("succeeded"))
                    {
                        //insert record in database
                    }
                    response = "charge.succeeded";
                    break;

                case "customer.subscription.updated":
                    // do work
                    response = "customer.subscription.updated";
                    break;
                case "customer.subscription.deleted":
                    // do work
                    response = "customer.subscription.deleted";
                    break;
                case "customer.subscription.created":
                    // do work
                    response = "customer.subscription.created";
                    break;
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK, response);
        }
    }
}