using Features.Payments.Stores;
using System;

namespace Features.Payments.Models
{
    /// <summary> Holds information about a payment request, a copy of it is stored in <see cref="PaymentStore"/> for future reference </summary>
    public class Payment
    {
        /// <summary> Initializes a new instance of the <see cref="Payment"/> class </summary>
        /// <param name="paymentRequest"> The payment requested by the merchant </param>
        /// <param name="bankResponse"> The acquiring bank response for the payment request </param>
        public Payment(PaymentRequest paymentRequest, BankResponse bankResponse)
        {
            if (paymentRequest == default)
                throw new ArgumentNullException(nameof(paymentRequest), "PaymentRequest must not be null");
            if (paymentRequest.CardNumber == default)
                throw new ArgumentNullException(nameof(paymentRequest), "Card number must not be null");
            if (paymentRequest.CardNumber.Length < 13)
                throw new ArgumentException(nameof(paymentRequest), "Card number is too short");
            if (bankResponse == default)
                throw new ArgumentNullException(nameof(bankResponse), "BankResponse must not be null");

            Key = Guid.NewGuid();

            MaskedCardNumber = $"****{paymentRequest.CardNumber.Substring(paymentRequest.CardNumber.Length - 4)}";
            ExpiryMonth = paymentRequest.ExpiryMonth;
            ExpiryYear = paymentRequest.ExpiryYear;
            AmountInMinorUnits = paymentRequest.Amount;
            Currency = paymentRequest.Currency;

            BankResponse = bankResponse;
        }

        /// <summary> Gets the identifier key for the payment, used by merchants to retrieve information about the payment </summary>
        public Guid Key { get; }

        /// <summary> Gets the masked card number of the card used </summary>
        /// <example> ****1234 </example>
        public string MaskedCardNumber { get; }

        /// <summary> Gets the expiry month of the card used </summary>
        /// <example> 10 </example>
        public int ExpiryMonth { get; }

        /// <summary> Gets the expiry year of the card usesd </summary>
        /// <example> 2030 </example>
        public int ExpiryYear { get; }

        /// <summary> Gets the amount of the payment in minor units </summary>
        /// <example> 199 </example>
        public long AmountInMinorUnits { get; }

        /// <summary> Gets currency in which the payment is made </summary>
        /// <example> GBP </example>
        public string Currency { get; }

        /// <summary> Gets the bank respones for the payment </summary>
        public BankResponse BankResponse { get; }
    }
}
