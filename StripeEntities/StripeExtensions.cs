using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Stripe;

namespace StripeEntities
{
    /// <summary>
    /// Helpful extension methods for classes in Stripe.Net
    /// </summary>
    public static class StripeExtensions
    {
        /// <summary>
        /// Gets the default card associated with this customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static StripeCard GetDefaultSource(this StripeCustomer customer)
        {
            if (customer.DefaultSource != null)
                return customer.DefaultSource;

            if (string.IsNullOrEmpty(customer.DefaultSourceId))
                return null;

            if (customer.SourceList == null || customer.SourceList.Data == null)
                return null;

            foreach(StripeCard card in customer.SourceList.Data)
            {
                if (card.Id == customer.DefaultSourceId)
                    return card;
            }

            return null;
        }
    }
}
