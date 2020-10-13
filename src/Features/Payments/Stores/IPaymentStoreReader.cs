using Features.Payments.Models;
using System;

namespace Features.Payments.Stores
{
    public interface IPaymentStoreReader
    {
        /// <summary> Retrieves the payment from store </summary>
        /// <param name="key"> The key with which the payment can be retrieved </param>
        Payment Retrieve(Guid key);
    }
}
