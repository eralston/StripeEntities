using System;

namespace StripeEntities
{
    /// <summary>
    /// Interface that defines an object's minimal properties to conveniently interact with the StripeManager
    /// </summary>
    public interface IChargeEntity
    {
        float Price { get; set; }
        string Title { get; set; }
    }
}
