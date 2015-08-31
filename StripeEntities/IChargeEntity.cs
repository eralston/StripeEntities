using System;

namespace StripeEntities
{
    /// <summary>
    /// Interface that defines an object's minimal properties to conveniently interact with the StripeManager
    /// This represents items like a product that can be purchased using Stripe in your system
    /// NOTE: This is NOT an IStripePersistentEntity - This object is NOT persisted in the Stripe API and is just a convenience
    /// </summary>
    public interface IChargeEntity
    {
        /// <summary>
        /// Gets or sets the price for this charge
        /// This is the price in USD (EG: 19.99 for something that is nineteen-dollars and ninety-nine cents)
        /// </summary>
        float Price { get; set; }

        /// <summary>
        /// Gets or sets the title for this charge
        /// This will appear in Stripe descritpion fields
        /// </summary>
        string Title { get; set; }
    }
}
