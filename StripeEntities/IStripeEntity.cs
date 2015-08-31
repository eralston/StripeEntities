using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StripeEntities
{
    /// <summary>
    /// A common interface for all objects that mirror entities persisted in the Stripe API
    /// </summary>
    public interface IStripePersistentEntity
    {
        /// <summary>
        /// Gets or sets the payment information associated with this user
        /// </summary>
        string PaymentSystemId { get; set; }
    }

    /// <summary>
    /// Extension methods on the IPaymentUser interface
    /// </summary>
    public static class IStripeUserExtensions
    {
        /// <summary>
        /// Returns true if the given user has payment information attached
        /// Otherwise, returns false
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool HasPaymentInfo(this IStripePersistentEntity user)
        {
            return !string.IsNullOrEmpty(user.PaymentSystemId);
        }
    }
}
