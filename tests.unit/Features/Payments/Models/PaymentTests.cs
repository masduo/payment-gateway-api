using Features.Payments.Models;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Integration.Features.Payments.Data
{
    [Collection("Models")]
    public class PaymentTests
    {
        [Fact]
        public void Ctor_ShouldThrowArgumentNullException_WhenPaymentRequestIsNull()
        {
            Action ctor = () => new Payment(
                paymentRequest: null,
                bankResponse: new BankResponse(default, false));

            ctor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_ShouldThrowArgumentNullException_WhenCardNumberIsNull()
        {
            Action ctor = () => new Payment(
                paymentRequest: new PaymentRequest { CardNumber = default },
                bankResponse: new BankResponse(default, false));

            ctor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_ShouldThrowArgumentException_WhenCardNumberIsTooShort_13_Characters()
        {
            Action ctor = () => new Payment(
                paymentRequest: new PaymentRequest { CardNumber = "too-short" },
                bankResponse: default);

            ctor.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_ShouldThrowArgumentNullException_WhenBankResponseIsNull()
        {
            Action ctor = () => new Payment(
                paymentRequest: new PaymentRequest { CardNumber = "not-too-short-anymore" },
                bankResponse: default);

            ctor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_ShouldGenerateKey()
        {
            var payment = new Payment(
                paymentRequest: new PaymentRequest { CardNumber = "not-too-short-anymore" },
                bankResponse: new BankResponse(default, false));

            payment.Key.Should().NotBe(default);
        }
    }
}
