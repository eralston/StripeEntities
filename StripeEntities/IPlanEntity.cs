﻿using System;
namespace StripeEntities
{
    /// <summary>
    /// Interface for an object that provides subscription plan information
    /// A "plan" is a reusable collection of price
    /// </summary>
    public interface IPlanEntity
    {
        string PaymentSystemId { get; set; }
        float Price { get; set; }
        string Title { get; set; }
        int TrialDays { get; set; }
    }
}
