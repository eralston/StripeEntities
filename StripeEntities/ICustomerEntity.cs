using System;

namespace StripeEntities
{
    /// <summary>
    /// An interface to be implemented by an object able to present data for Stripe customers
    /// This is often implemented by the same object that identifies authenticated users in your system
    /// </summary>
    public interface ICustomerEntity : IStripePersistentEntity
    {
        /// <summary>
        /// Gets or sets the e-mail address for this user, which is used to uniquely identify them in the payment system
        /// </summary>
        string Email { get; set; }
    }
}
