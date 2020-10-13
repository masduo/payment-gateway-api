using Features.Payments.Models;
using Features.Payments.Services;
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

namespace Tests.Integration.Features.Payments.Models
{
    [Collection("Models")]
    public class PaymentRequestTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public PaymentRequestTests(WebApplicationFactory<Startup> factory)
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

        [Fact]
        public async Task Process_ShouldReturnOk_WhenPayloadIsValid()
        {
            var payload = Helpers.GetValidPaymentRequest();

            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(payload));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be("application/json; charset=utf-8");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Process_ShouldReturnBadRequest_WhenPayloadIsNullOrEmptyOrWhitespace(string payload)
        {
            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(payload));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Process_ShouldReturnBadRequest_WhenPayloadIsNotJson()
        {
            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent("{defo-not-json}"));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Process_ShouldReturnBadRequest_WhenCardNumberIsNullOrEmptyOrWhitespace(string cardNumber)
        {
            var payload = Helpers.GetValidPaymentRequest();
            payload.CardNumber = cardNumber;

            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(payload));
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain(nameof(PaymentRequest.CardNumber));
        }

        [Fact]
        public async Task Process_ShouldReturnBadRequest_WhenCardNumberIsShorterThan_13_characters()
        {
            var payload = Helpers.GetValidPaymentRequest();
            payload.CardNumber = "123456789012";

            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(payload));
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain(nameof(PaymentRequest.CardNumber));
        }

        [Theory]
        [InlineData(default(int))]
        [InlineData(-1)]
        [InlineData(13)]
        public async Task Process_ShouldReturnBadRequest_WhenExpiryMonthIsOutsideRange_1_to_12(int expiryMonth)
        {
            var payload = Helpers.GetValidPaymentRequest();
            payload.ExpiryMonth = expiryMonth;

            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(payload));
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain(nameof(PaymentRequest.ExpiryMonth));
        }

        [Theory]
        [InlineData(default(int))]
        [InlineData(2019)]
        [InlineData(2121)]
        public async Task Process_ShouldReturnBadRequest_WhenExpiryYearIsOutsideRange_2020_to_2120(int expiryYear)
        {
            var payload = Helpers.GetValidPaymentRequest();
            payload.ExpiryYear = expiryYear;

            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(payload));
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain(nameof(PaymentRequest.ExpiryYear));
        }

        [Fact]
        public async Task Process_ShouldReturnBadRequest_WhenExpiryDateIsInThePast()
        {
            var payload = Helpers.GetValidPaymentRequest();
            payload.ExpiryYear = DateTime.UtcNow.Year;
            payload.ExpiryMonth = DateTime.UtcNow.Month - 1;

            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(payload));
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain(nameof(PaymentRequest.ExpiryMonth));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Process_ShouldReturnBadRequest_WhenCvvIsNullOrEmptyOrWhitespace(string cvv)
        {
            var payload = Helpers.GetValidPaymentRequest();
            payload.Cvv = cvv;

            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(payload));
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain(nameof(PaymentRequest.Cvv));
        }

        [Theory]
        [InlineData("12")]
        [InlineData("12345")]
        public async Task Process_ShouldReturnBadRequest_WhenCvvIsShorterThan_3_orLongerThan_4_characters(string cvv)
        {
            var payload = Helpers.GetValidPaymentRequest();
            payload.Cvv = cvv;

            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(payload));
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain(nameof(PaymentRequest.Cvv));
        }

        [Theory]
        [InlineData(default(long))]
        [InlineData(-1)]
        public async Task Process_ShouldReturnBadRequest_WhenAmountIsZero(long amount)
        {
            var payload = Helpers.GetValidPaymentRequest();
            payload.Amount = amount;

            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(payload));
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain(nameof(PaymentRequest.Amount));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Process_ShouldReturnBadRequest_WhenCurrencyIsNullOrEmptyOrWhitespace(string currency)
        {
            var payload = Helpers.GetValidPaymentRequest();
            payload.Currency = currency;

            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(payload));
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain(nameof(PaymentRequest.Currency));
        }

        [Theory]
        [InlineData("GB")]
        [InlineData("GBPP")]
        public async Task Process_ShouldReturnBadRequest_WhenCurrencyIsNotThreeCharactersLong(string currency)
        {
            var payload = Helpers.GetValidPaymentRequest();
            payload.Currency = currency;

            using var response = await _client.PostAsync(Helpers.PaymentsResourceUrlV1, Helpers.GetStringContent(payload));
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain(nameof(PaymentRequest.Currency));
        }
    }
}
