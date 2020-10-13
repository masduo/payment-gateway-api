using System;
using System.ComponentModel.DataAnnotations;

namespace Features.Payments.Models
{
    /// <summary> The request payload for <see cref="PaymentsController.Process(PaymentRequest)"/> endpoint </summary>
    public class PaymentRequest
    {
        /// <summary> Gets or sets the card number to pay with </summary>
        /// <example> 1234-1234-1234-1234 </example>
        [Required]
        [MinLength(13)]
        public string CardNumber { get; set; }

        /// <summary> Gets or sets an integral value for the expiry month of the card </summary>
        /// <example> 10 </example>
        [Required]
        [Range(1, 12)]
        public int ExpiryMonth { get; set; }

        /// <summary> Gets or sets an integral value for the expiry year of the card </summary>
        /// <example> 2030 </example>
        [Required]
        [Range(2020, 2120)]
        public int ExpiryYear { get; set; }

        /// <summary> Gets or sets the 3 or 4 digits value representing the card verification value </summary>
        /// <example> 123 </example>
        [Required]
        [MinLength(3), MaxLength(4)]
        public string Cvv { get; set; }

        /// <summary> Gets or sets an integral value for the payment amount in minor units, e.g. `199` represents `1.99`, and `1` represent `.01` </summary>
        /// <example> 199 </example>
        [Required]
        [Range(1, long.MaxValue)]
        public long Amount { get; set; }

        /// <summary> Gets or sets the 3 character currency code </summary>
        /// <example> GBP </example>
        [Required]
        [MinLength(3), MaxLength(3)]
        public string Currency { get; set; }
    }
}
