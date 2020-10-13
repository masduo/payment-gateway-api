using Features.Payments.Services;
using Features.Payments.Stores;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using PaymentGateway.Api;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tests.Integration.Features.Payments.Fakes;
using Xunit;

namespace Tests.Integration.Features.Payments
{
    [Collection("Controller")]
    public class PaymentsControllerRetrieveTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public PaymentsControllerRetrieveTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services
                        .AddTransient<IPaymentStoreWriter, FakePaymentStore>()
                        .AddTransient<IBankService, FakeBankService>();
                }))
                .CreateClient();
        }

        [Fact]
        public async Task Process_ShouldRetrunOk_WhenValidPaymentRequestIsUsed()
        {
            using var response = await _client.PostAsync($"{Helpers.PaymentsResourceUrlV1}", Helpers.GetStringContent(Helpers.GetValidPaymentRequest()));
            var content = await response.Content.ReadAsStringAsync();
            dynamic deserialized = JObject.Parse(content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            bool status = deserialized.payment.bankResponse.status;
            status.Should().Be(true);
        }

        [Fact]
        public async Task Process_ShouldRetrunOk_WhenFailingCardNumberIsUsed()
        {
            var payload = Helpers.GetValidPaymentRequest();
            payload.CardNumber = FakeBankService.FailingCardNumber;

            using var response = await _client.PostAsync($"{Helpers.PaymentsResourceUrlV1}", Helpers.GetStringContent(payload));
            var content = await response.Content.ReadAsStringAsync();
            dynamic deserialized = JObject.Parse(content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            bool status = deserialized.payment.bankResponse.status;
            status.Should().Be(false);
        }
    }
}