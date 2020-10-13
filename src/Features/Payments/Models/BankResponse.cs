using Features.Payments.Services;
using System;

namespace Features.Payments.Models
{
    /// <summary> The response for a payment request from the acquiring bank <see cref="IBankService"/> </summary>
    public class BankResponse
    {
        /// <summary> Initializes a new instance of the <see cref="BankResponse"/> class </summary>
        /// <param name="reference"> The unique identifier for the payment request </param>
        /// <param name="status"> The status of the payment request, `true` indicates success </param>
        public BankResponse(Guid reference, bool status)
        {
            Reference = reference;
            Status = status;
        }

        /// <summary> Gets the unique identifier for the payment request </summary>
        public Guid Reference { get; }

        /// <summary> Gets a value indicating whether the status of the payment request was successfully processed or not, `true` indicates success </summary>
        public bool Status { get; }
    }
}
