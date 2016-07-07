using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace StripeEntities
{
    /// <summary>
    /// Describes a product in the system, a repeatable type of transaction one can make
    /// </summary>
    public class Product : IChargeEntity
    {
        /// <summary>
        /// Enumeration for the possible states of a subscription
        /// </summary>
        public enum ProductState
        {
            /// <summary>
            /// State indicating product is in the system, but not yet available
            /// </summary>
            Pending,

            /// <summary>
            /// State indicating the product is entered and available in the system
            /// </summary>
            Available,

            /// <summary>
            /// State indicating the product is in the system, but no longer available
            /// </summary>
            Retired
        }

        /// <summary>
        /// Gets or sets the state of this product, dictating if it's available
        /// </summary>
        [DefaultValue(ProductState.Pending)]
        [Editable(true)]
        public ProductState State { get; set; }

        /// <summary>
        /// Gets or sets the title for this product
        /// This is a required field
        /// </summary>
        [Required]
        [Editable(true)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description for this product
        /// </summary>
        [Editable(true)]
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the price for this product
        /// EG: 99.99 (ninety-nine dollars and ninety-nine cents)
        /// </summary>
        [Editable(false)]
        public float Price { get; set; }
    }
}
