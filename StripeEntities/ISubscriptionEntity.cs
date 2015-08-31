using System;
namespace StripeEntities
{
    /// <summary>
    /// Interface for an object providing data storage for subscriptions (links from customers to plans)
    /// </summary>
    public interface ISubscriptionEntity
    {
        string PaymentSystemId { get; set; }
    }
}
