using Features.Payments.Models;
using Features.Payments.Stores;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Tests.Integration.Features.Payments.Data
{
    [Collection("Stores")]
    public class PaymentStoreTests
    {
        [Fact]
        public void Add_ShouldThrowArgumentNullException_WhenPaymentIsNull()
        {
            Action add = () => new PaymentStore(logger: null)
                .Add(payment: null);

            add.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Add_ShouldThrowInvalidOperationException_WhenPaymentIsAlreadyAdded()
        {
            var logger = new Mock<ILogger<PaymentStore>>();
            var paymentStore = new PaymentStore(logger.Object);

            var paymentRequest = new PaymentRequest() { CardNumber = "1234_1234_1234_1234" };
            var bankResponse = new BankResponse(reference: default, status: default);
            var payment = new Payment(paymentRequest, bankResponse);

            // add the same payment twice
            paymentStore.Add(payment);
            Action addAgain = () => paymentStore.Add(payment);

            addAgain.Should().Throw<InvalidOperationException>();
        }
    }
}
