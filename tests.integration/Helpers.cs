using Features.Payments.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Tests.Integration
{
    public class Helpers
    {
        public const string PaymentsResourceUrlV1 = "/v1/payments";

        public static StringContent GetStringContent(object payload) =>
            new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8)
            {
                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
            };

        public static PaymentRequest GetValidPaymentRequest() =>
            new PaymentRequest
            {
                CardNumber = "1234 1234 1234 1234",
                ExpiryMonth = 12,
                ExpiryYear = 2030,
                Cvv = "123",
                Amount = 199,
                Currency = "GBP"
            };
    }
}
