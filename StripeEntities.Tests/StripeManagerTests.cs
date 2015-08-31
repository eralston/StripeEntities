using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Stripe;

namespace StripeEntities.Tests
{
    [TestClass]
    public class StripeManagerTests
    {
        #region Helper Methods Methods

        const string TestPlanA_Id = "123456";
        const string TestPlanB_Id = "7890";

        /// <summary>
        /// Creates a new mock object for a fixed test subscription plan
        /// </summary>
        /// <returns></returns>
        public static IPlanEntity CreateMockPlanA()
        {
            var planMock = new Mock<IPlanEntity>();

            planMock.SetupAllProperties();

            IPlanEntity plan = planMock.Object;

            plan.Title = "Plan A";
            plan.Price = 19.99f;
            plan.TrialDays = 13;
            plan.PaymentSystemId = TestPlanA_Id;

            return plan;
        }

        /// <summary>
        /// Creates an alternative mock test plan
        /// </summary>
        /// <returns></returns>
        public static IPlanEntity CreateMockPlanB()
        {
            var planMock = new Mock<IPlanEntity>();

            planMock.SetupAllProperties();

            IPlanEntity plan = planMock.Object;

            plan.Title = "Plan B";
            plan.Price = 50;
            plan.TrialDays = 8;
            plan.PaymentSystemId = TestPlanB_Id;

            return plan;
        }

        /// <summary>
        /// Creates a new mock object for a user in the system
        /// </summary>
        /// <returns></returns>
        public static ICustomerEntity CreateMockCustomer()
        {
            var userMock = new Mock<ICustomerEntity>();

            userMock.SetupAllProperties();
            userMock.Object.Email = "erik@code.com";

            return userMock.Object;
        }

        /// <summary>
        /// Creates a new mock object for a subscription in the plan
        /// </summary>
        /// <returns></returns>
        public static ISubscriptionEntity CreateMockSubscription()
        {
            var subMock = new Mock<ISubscriptionEntity>();

            subMock.SetupAllProperties();

            return subMock.Object;
        }

        public static IChargeEntity CreateMockCharge()
        {
            var subMock = new Mock<IChargeEntity>();

            subMock.SetupAllProperties();

            subMock.Object.Price = 9.99f;
            subMock.Object.Title = "Charge with customer and charge entity";

            return subMock.Object;
        }

        /// <summary>
        /// Creates a token as per 
        /// https://github.com/jaymedavis/stripe.net#creating-a-token
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static StripeToken CreateTestToken(ICustomerEntity customer = null)
        {
            var myToken = new StripeTokenCreateOptions();

            // if you need this...
            myToken.Card = new StripeCreditCardOptions()
            {
                // set these properties if passing full card details (do not
                // set these properties if you set TokenId)
                Number = "4242424242424242",
                ExpirationYear = "2063",
                ExpirationMonth = "10",
                AddressCountry = "US",                // optional
                AddressLine1 = "24 Beef Flank St",    // optional
                AddressLine2 = "Apt 24",              // optional
                AddressCity = "Biggie Smalls",        // optional
                AddressState = "NC",                  // optional
                AddressZip = "27617",                 // optional
                Name = "Joe Meatballs",               // optional
                Cvc = "1223"                          // optional
            };

            if (customer != null)
                myToken.CustomerId = customer.PaymentSystemId;

            var tokenService = new StripeTokenService();
            StripeToken stripeToken = tokenService.Create(myToken);
            return stripeToken;
        }

        /// <summary>
        /// Method that deletes the test plan
        /// This is used both during initialization and for the unit testing of plan delete
        /// </summary>
        private static void DeleteTestPlanA()
        {
            // Arrange
            var plan = CreateMockPlanA();
            // Act
            StripeManager.DeletePlan(plan);
        }

        private static void DeleteTestPlanB()
        {
            // Arrange
            var plan = CreateMockPlanB();
            // Act
            StripeManager.DeletePlan(plan);
        }

        public static void EnsureTestPlansDeleted()
        {
            try
            {
                DeleteTestPlanA();
                DeleteTestPlanB();
            }
            catch { }
        }

        #endregion

        #region Test Methods

        [TestMethod]
        public void TestPlans()
        {
            // Arrange
            EnsureTestPlansDeleted();
            IPlanEntity plan = CreateMockPlanA();

            // Act - create
            StripePlan createdPlan = StripeManager.CreatePlan(plan);

            // Assert - create
            Assert.IsNotNull(createdPlan);

            // Act - update
            plan.Title = "Unit Test Plan - Name Changed";
            StripePlan updatedPlan = StripeManager.UpdatePlan(plan);

            // Assert - update
            Assert.IsNotNull(updatedPlan);

            // Act - Delete
            StripeManager.DeletePlan(plan);

            // Assert
            try
            {
                StripePlanService planService = new StripePlanService();
                planService.Get(TestPlanA_Id);
                Assert.Fail(); // We should not get to this line
            }
            catch (Exception ex)
            {
                // We should get an exception that says "No such plan"
                Assert.IsTrue(ex.Message.Contains("No such plan"));
            }
        }

        [TestMethod]
        public void TestCustomers()
        {
            // SUBSCRIBE

            // Arrange
            ICustomerEntity customer = CreateMockCustomer();

            // Act
            StripeManager.CreateCustomer(customer);

            // Assert 
            Assert.IsNotNull(customer.PaymentSystemId);

            // RETRIEVE

            // Act
            StripeCustomer stripeCustomer = StripeManager.RetrieveCustomer(customer);

            // Assert
            Assert.IsNotNull(stripeCustomer);

            // UPDATE

            // Arrange
            customer.Email = "UpdateCustomer@code.com";

            // act
            StripeCustomer updatedStripeCustomer = StripeManager.UpdateCustomer(customer);

            // Assert
            Assert.IsNotNull(updatedStripeCustomer);
        }

        [TestMethod]
        public void TestSubscriptions()
        {
            // Arrange
            EnsureTestPlansDeleted();

            // NOTE: Due to the reliance on the API, we must create these for real
            IPlanEntity planA = CreateMockPlanA();
            StripeManager.CreatePlan(planA);

            ICustomerEntity customer = CreateMockCustomer();
            StripeManager.CreateCustomer(customer);

            ISubscriptionEntity subscription = CreateMockSubscription();

            // CREATE
            // Subscribe

            // Act
            StripeSubscription newSub = StripeManager.Subscribe(customer, subscription, planA);
            Assert.IsNotNull(newSub);

            // CHANGE
            // ChangeSubscriptionPlan

            IPlanEntity planB = CreateMockPlanB();
            StripeManager.CreatePlan(planB);

            StripeSubscription changedSub = StripeManager.ChangeSubscriptionPlan(customer, subscription, planB);
            Assert.IsNotNull(changedSub);

            // DELETE
            StripeSubscription cancelledSub = StripeManager.Unsubscribe(customer, subscription);
            Assert.IsNotNull(cancelledSub);
            Assert.IsTrue(cancelledSub.Status == "canceled");
        }

        public void TestCharge()
        {
            StripeToken token = CreateTestToken();
            string chargeId = StripeManager.Charge(token.Id, 78.90f, "Test naked charge");
            Assert.IsNotNull(chargeId);
        }


        [TestMethod]
        public void TestChargeWithCustomer()
        {
            // Arrange
            ICustomerEntity customer = CreateMockCustomer();
            IChargeEntity charge = CreateMockCharge();

            StripeToken token = CreateTestToken(customer);
            StripeManager.CreateCustomer(customer, token.Id);

            // Act - charge customer
            string chargeId = StripeManager.Charge(customer, charge);
            Assert.IsNotNull(chargeId);

            chargeId = StripeManager.Charge(customer, 12.34f, "Test charge with customer");
            Assert.IsNotNull(chargeId);
        }
        
        #endregion
    }
}
