using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using Masticore;

namespace StripeEntities
{
    /// <summary>
    /// A model for capturing available subscription plans in the system
    /// There should be one of these for each pricing/service tier in the system
    /// These are mirrored into the billing system by API integration
    /// </summary>
    public class PlanEntity : ModelBase, StripeEntities.IPlanEntity
    {
        /// <summary>
        /// Enumeration for the possible states of a subscription
        /// </summary>
        public enum SubscriptionState
        {
            Pending,
            Available,
            Retired
        }

        [Editable(true)]
        [Required]
        public virtual string Title { get; set; }

        /// <summary>
        /// The identifier used over in Stripe for this plan
        /// </summary>
        [Editable(false)]
        public string PaymentSystemId { get; set; }

        [Editable(true)]
        public string Note { get; set; }

        [Editable(true)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Trial Days")]
        [DefaultValue(0)]
        [Editable(false)]
        public int TrialDays { get; set; }

        [Editable(false)]
        public float Price { get; set; }

        [Editable(true)]
        [DefaultValue(SubscriptionState.Available)]
        public SubscriptionState State { get; set; }
    }
}