using System;
namespace StripeEntities
{
    /// <summary>
    /// Interface for an object that provides subscription plan information
    /// </summary>
    public interface IPlanEntity : IStripePersistentEntity
    {
        /// <summary>
        /// Gets or sets the price in USD for this plan
        /// </summary>
        float Price { get; set; }

        /// <summary>
        /// Gets or sets the title for this plan
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the trial days for this plan
        /// </summary>
        int TrialDays { get; set; }
    }

    /// <summary>
    /// Extension methods for IPlanEntity
    /// </summary>
    public static class IPlanEntityExtensions
    {
        /// <summary>
        /// Generates a new, unique payment system ID for this plan
        /// NOTE: Should only be called once when creating the plan and only if there isn't a better human-readable ID in your system
        /// </summary>
        /// <param name="plan"></param>
        public static void GeneratePaymentSystemId(this IPlanEntity plan)
        {
            plan.PaymentSystemId = Guid.NewGuid().ToString("N");
        }
    }
}
