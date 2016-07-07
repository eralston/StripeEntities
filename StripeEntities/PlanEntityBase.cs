using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace StripeEntities
{
    /// <summary>
    /// A model for capturing available subscription plans in the system
    /// There should be one of these for each pricing/service tier in the system
    /// These are mirrored into the billing system by API integration
    /// </summary>
    public abstract class PlanEntityBase : IPlanEntity
    {
        /// <summary>
        /// Enumeration for the possible states of a subscription
        /// </summary>
        public enum PlanState
        {
            /// <summary>
            /// Indicates the plan is entered, but not yet available
            /// </summary>
            Pending,

            /// <summary>
            /// Indicates the plan is entered and available
            /// </summary>
            Available,
            
            /// <summary>
            /// Indicate the plan was once available, but is no longer 
            /// </summary>
            Retired
        }

        /// <summary>
        /// Gets or sets the title for this plan
        /// </summary>
        [Editable(true)]
        [Required]
        public virtual string Title { get; set; }

        /// <summary>
        /// The identifier used over in Stripe for this plan
        /// NOTE: This must be set for the plan before it can be created in Stripe
        /// Once set, this should NEVER be modified without destroying the plan in Stripe first
        /// </summary>
        [Editable(false)]
        public virtual string PaymentSystemId { get; set; }

        /// <summary>
        /// Gets or sets the number of trial days available on this plan
        /// </summary>
        [DisplayName("Trial Days")]
        [DefaultValue(0)]
        [Editable(false)]
        public virtual int TrialDays { get; set; }

        /// <summary>
        /// Gets or sets the price in USD for this plan
        /// </summary>
        [Editable(false)]
        public virtual float Price { get; set; }

        /// <summary>
        /// Gets or sets the state for this plan
        /// </summary>
        [Editable(true)]
        [DefaultValue(PlanState.Available)]
        public virtual PlanState State { get; set; }
    }
}