using Features.Payments.Stores;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tests.Integration.Features.Payments.Fakes;
using Xunit;

namespace Tests.Integration.Features.Payments
{
    [Collection("Controller")]
    public class PaymentsControllerProcessTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public PaymentsControllerProcessTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services.AddTransient<IPaymentStoreReader, FakePaymentStore>();
                }))
                .CreateClient();
        }

        [Fact]
        public async Task Retrieve_ShouldRetrunMethodNotAllowed_WhenNoKeyIsSetInRoute()
        {
            using var response = await _client.GetAsync($"{Helpers.PaymentsResourceUrlV1}/");

            response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Theory]
        [InlineData("-")]
        [InlineData("not-guid")]
        [InlineData("123-123")]
        public async Task Retrieve_ShouldRetrunBadRequest_WhenKeyIsNotGuid(string nonGuidKey)
        {
            using var response = await _client.GetAsync($"{Helpers.PaymentsResourceUrlV1}/{nonGuidKey}");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Retrieve_ShouldRetrunBadRequest_WhenKeyIsDefault()
        {
            using var response = await _client.GetAsync($"{Helpers.PaymentsResourceUrlV1}/{default(Guid)}");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Retrieve_ShouldRetrunOk_WhenFakeKeyIsUsed()
        {
            var fakeStoredKey = Guid.Parse(FakePaymentStore.FakeKeyThatExistsInStore);

            using var response = await _client.GetAsync($"{Helpers.PaymentsResourceUrlV1}/{fakeStoredKey}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Retrieve_ShouldRetrunOk_WhenRandomKeyIsUsed()
        {
            var randomKey = Guid.NewGuid();

            using var response = await _client.GetAsync($"{Helpers.PaymentsResourceUrlV1}/{randomKey}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}