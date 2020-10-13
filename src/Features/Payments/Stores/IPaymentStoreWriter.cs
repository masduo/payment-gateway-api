using Features.Payments.Models;

namespace Features.Payments.Stores
{
    public interface IPaymentStoreWriter
    {
        /// <summary> Adds the payment to store </summary>
        /// <param name="payment"> The payment to be stored </param>
        void Add(Payment payment);
    }
}
