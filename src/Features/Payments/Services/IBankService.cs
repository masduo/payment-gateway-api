using Features.Payments.Models;

namespace Features.Payments.Services
{
    public interface IBankService
    {
        /// <summary> Send payment request to acquiring bank to process </summary>
        /// <param name="paymentRequest"> The payment request </param>
        BankResponse Process(PaymentRequest paymentRequest);
    }
}
