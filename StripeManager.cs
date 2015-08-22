using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StripeEntities
{
    /// <summary>
    /// Wraps all of the functionality for stripe into a helper class taking only models as input and out
    /// Uses Stripe.Net https://github.com/jaymedavis/stripe.net
    /// This should make converting this functionality into another technology easier in the future
    /// 
    /// 1) Create a plan
    /// 2) Create a user (with payment token)
    /// 3) Subscribe a user to a plan
    /// </summary>
    public class StripeManager
    {
        #region Plans

        /// <summary>
        /// Creates a new plan inside of Stripe, using the given subscription plan's information
        /// </summary>
        /// <param name="plan"></param>
        public static void CreatePlan(IStripeSubscriptionPlan plan)
        {
            // Save it to Stripe
            StripePlanCreateOptions newStripePlanOptions = new StripePlanCreateOptions();
            newStripePlanOptions.Amount = Convert.ToInt32(plan.Price * 100.0); // all amounts on Stripe are in cents, pence, etc
            newStripePlanOptions.Currency = "usd";                                 // "usd" only supported right now
            newStripePlanOptions.Interval = "month";                               // "month" or "year"
            newStripePlanOptions.IntervalCount = 1;                                // optional
            newStripePlanOptions.Name = plan.Title;
            newStripePlanOptions.TrialPeriodDays = plan.TrialDays;     // amount of time that will lapse before the customer is billed
            newStripePlanOptions.Id = plan.PaymentSystemId;

            StripePlanService planService = new StripePlanService();
            StripePlan newPlan = planService.Create(newStripePlanOptions);
            plan.PaymentSystemId = newPlan.Id;

            System.Diagnostics.Trace.TraceInformation("Created new plan in stripe: '{0}' with id {1}", plan.Title, plan.PaymentSystemId);
        }

        /// <summary>
        /// Updates the given plan
        /// NOTE: Due to limitatons with Stripe, this can only update the name of the plan
        /// </summary>
        /// <param name="plan"></param>
        public static void UpdatePlan(IStripeSubscriptionPlan plan)
        {
            StripePlanUpdateOptions options = new StripePlanUpdateOptions();
            options.Name = plan.Title;

            StripePlanService planService = new StripePlanService();
            planService.Update(plan.PaymentSystemId, options);

            System.Diagnostics.Trace.TraceInformation("Updated plan in stripe: '{0}' with id '{1}'", plan.Title, plan.PaymentSystemId);
        }

        /// <summary>
        /// Deletes a plan from Stripe
        /// NOTE: Delete the model from the underlying context after calling this method
        /// </summary>
        /// <param name="plan"></param>
        public static void DeletePlan(IStripeSubscriptionPlan plan)
        {
            var planService = new StripePlanService();
            planService.Delete(plan.PaymentSystemId);

            System.Diagnostics.Trace.TraceInformation("Deleting plan in stripe: '{0}' with id '{1}", plan.Title, plan.PaymentSystemId);

            plan.PaymentSystemId = null;
        }

        #endregion

        #region Customer

        /// <summary>
        /// Creates a new customer record in Stripe for the given user
        /// NOTE: Save changes on the underlying context for the model after calling this method
        /// </summary>
        /// <param name="user"></param>
        public static void CreateCustomer(IStripeUser user, string paymentToken = null)
        {
            // Do not overwrite the user, ever
            if (user.HasPaymentInfo())
                return;

            var newCustomer = new StripeCustomerCreateOptions();

            newCustomer.Email = user.Email;

            if (paymentToken != null)
                newCustomer.Source = new StripeSourceOptions() { TokenId = paymentToken };

            var customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = customerService.Create(newCustomer);

            // Set the accounting info
            user.PaymentSystemId = stripeCustomer.Id;

            System.Diagnostics.Trace.TraceInformation("Created customer in stripe: '{0}' with id '{1}", user.Email, user.PaymentSystemId);
        }

        /// <summary>
        /// Retrieves the StripeCustomer associated with the given IStripeUser instance
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static StripeCustomer RetrieveCustomer(IStripeUser user)
        {
            var customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = customerService.Get(user.PaymentSystemId);
            return stripeCustomer;
        }

        /// <summary>
        /// Updates a customer record, using the given payment token
        /// NOTE: Save changes on the underlying context for the model after calling this method
        /// </summary>
        /// <param name="user"></param>
        /// <param name="paymentToken"></param>
        public static void UpdateCustomer(IStripeUser user, string paymentToken = null)
        {
            var customerUpdate = new StripeCustomerUpdateOptions() { Email = user.Email };

            // Create a token for this payment token
            customerUpdate.Source = new StripeSourceOptions() { TokenId = paymentToken };

            var customerService = new StripeCustomerService();
            StripeCustomer stripeCustomer = customerService.Update(user.PaymentSystemId, customerUpdate);

            System.Diagnostics.Trace.TraceInformation("Updated customer in stripe: '{0}' with id '{1}", user.Email, user.PaymentSystemId);
        }

        /// <summary>
        /// Creates or update a customer
        /// </summary>
        /// <param name="user"></param>
        /// <param name="paymentToken"></param>
        public static void CreateOrUpdateCustomer(IStripeUser user, string paymentToken = null)
        {
            if (user.HasPaymentInfo())
                UpdateCustomer(user, paymentToken);
            else
                CreateCustomer(user, paymentToken);
        }

        #endregion

        #region Subscribing Customers to Plans

        /// <summary>
        /// Subscribes the given user to the given plan, using the payment information already in stripe for that user
        /// NOTE: Save changes on the underlying context for the model after calling this method
        /// </summary>
        /// <param name="subscription"></param>
        public static void Subscribe(IStripeUser user, IStripeSubscription subscription, IStripeSubscriptionPlan plan)
        {
            if (!string.IsNullOrEmpty(subscription.PaymentSystemId))
                return;

            var subscriptionService = new StripeSubscriptionService();
            StripeSubscription stripeSubscription = subscriptionService.Create(user.PaymentSystemId, plan.PaymentSystemId);
            subscription.PaymentSystemId = stripeSubscription.Id;

            System.Diagnostics.Trace.TraceInformation("Subscribed customer in stripe: '{0}' with new subscription id '{1}", user.Email, subscription.PaymentSystemId);
        }

        /// <summary>
        /// Changes the given subscription to use the new plan
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="newPlan"></param>
        public static void ChangeSubscriptionPlan(IStripeUser user, IStripeSubscription subscription, IStripeSubscriptionPlan newPlan)
        {
            StripeSubscriptionUpdateOptions options = new StripeSubscriptionUpdateOptions() { PlanId = newPlan.PaymentSystemId };

            var subscriptionService = new StripeSubscriptionService();
            subscriptionService.Update(user.PaymentSystemId, subscription.PaymentSystemId, options);

            System.Diagnostics.Trace.TraceInformation("Changed subscription for customer in stripe: '{0}' with new subscription id '{1}", user.Email, subscription.PaymentSystemId);
        }

        /// <summary>
        /// Unsubscribes the given subscription
        /// NOTE: Save changes on the underlying context for the model after calling this method
        /// </summary>
        /// <param name="subscription"></param>
        public static void Unsubscribe(IStripeUser user, IStripeSubscription subscription)
        {
            if (string.IsNullOrEmpty(subscription.PaymentSystemId) || string.IsNullOrEmpty(user.PaymentSystemId))
                return;

            var subscriptionService = new StripeSubscriptionService();
            subscriptionService.Cancel(subscription.PaymentSystemId, user.PaymentSystemId);
            subscription.PaymentSystemId = null;

            System.Diagnostics.Trace.TraceInformation("Unsuscribed customer in stripe: '{0}' with new subscription id '{1}", user.Email, subscription.PaymentSystemId);
        }

        #endregion

        #region Products & Transactions

        /// <summary>
        /// Charges the given user one time for the given price in USD
        /// </summary>
        /// <param name="user"></param>
        /// <param name="price"></param>
        /// <param name="chargeDescription"></param>
        /// <returns></returns>
        public static string Charge(IStripeUser user, float price, string chargeDescription = "")
        {
            var charge = CreateChargeOptions(price, chargeDescription);

            // setting up the card
            charge.CustomerId = user.PaymentSystemId;

            return ExecuteCharge(charge);
        }

        /// <summary>
        /// Charges the given card token one time for the given price in USD
        /// </summary>
        /// <param name="cardToken"></param>
        /// <param name="price"></param>
        /// <param name="chargeDescription"></param>
        /// <returns></returns>
        public static string Charge(string cardToken, float price, string chargeDescription = "")
        {
            var charge = CreateChargeOptions(price, chargeDescription);

            // setting up the card
            charge.Source = new StripeSourceOptions()
            {
                // set this property if using a token
                TokenId = cardToken
            };

            return ExecuteCharge(charge);
        }

        /// <summary>
        /// Charges the given user for the given product
        /// </summary>
        /// <param name="user"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        public static string Charge(IStripeUser user, IStripeProduct product)
        {
            return Charge(user, product.Price, product.Title);
        }

        /// <summary>
        /// Creates a new charge option instance, initializing it with common properties
        /// </summary>
        /// <param name="price"></param>
        /// <param name="chargeDescription"></param>
        /// <returns></returns>
        private static StripeChargeCreateOptions CreateChargeOptions(float price, string chargeDescription)
        {
            var charge = new StripeChargeCreateOptions();

            // always set these properties
            charge.Amount = Convert.ToInt32(price * 100.0);
            charge.Currency = "usd";

            // set this if you want to
            charge.Description = chargeDescription;

            // (not required) set this to false if you don't want to capture the charge yet - requires you call capture later
            charge.Capture = true;

            return charge;
        }

        /// <summary>
        /// Executes the given charge options, returning the ID for the charge
        /// </summary>
        /// <param name="charge"></param>
        /// <returns></returns>
        private static string ExecuteCharge(StripeChargeCreateOptions charge)
        {
            var chargeService = new StripeChargeService();
            StripeCharge stripeCharge = chargeService.Create(charge);

            // Log the charge
            System.Diagnostics.Trace.TraceInformation("Created new charge in stripe: '{0}' for {1}",
                charge.Description,
                charge.Amount);

            // Return the ID for the charge
            return stripeCharge.Id;
        }

        #endregion
    }
}