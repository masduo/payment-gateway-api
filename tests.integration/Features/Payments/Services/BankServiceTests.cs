using Features.Payments.Models;
using Features.Payments.Services;
using Features.Payments.Stores;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tests.Integration.Features.Payments.Fakes;
using Xunit;

namespace Tests.Integration
{
    [Collection("Services")]
    public class BankServiceTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public BankServiceTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services
                        .AddTransient<IBankService, FakeBankService>()
                        .AddTransient<IPaymentStoreWriter, FakePaymentStore>()
                        .AddTransient<IPaymentStoreReader, FakePaymentStore>();
                }))
                .CreateClient();
        }

        private PaymentRequest getValidPaymentRequest() =>
            new PaymentRequest
            {
                CardNumber = "1234 1234 1234 1234",
                ExpiryMonth = 12,
                ExpiryYear = 2030,
                Cvv = "123",
                Amount = 199,
                Currency = "GBP"
            };

        [Fact]
        public async Task Process_ShouldReturnOk_WhenPaymentRequestSucceed()
        {
            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(getValidPaymentRequest()));
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
