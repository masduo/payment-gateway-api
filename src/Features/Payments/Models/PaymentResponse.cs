using Shared.Models;
using System.Text.Json.Serialization;

namespace Features.Payments.Models
{
    /// <summary> The response payload for Process Payment <see cref="PaymentsController.Process(PaymentRequest)"/> endpoint </summary>
    public class PaymentResponse
    {
        /// <summary> Gets or sets the payment data including the request and bank response </summary>
        public Payment Payment { get; set; }

        /// <summary> Gets an array of hypermedia links to instruct the caller of next possible functions on the payment resource</summary>
        [JsonPropertyName("_links")]
        public Link[] Links
        {
            get => new[]
            {
                new Link { Href = $"/v1/payments/{Payment.Key}", Rel = "self", Method = "GET" }
            };
        }
    }
}
