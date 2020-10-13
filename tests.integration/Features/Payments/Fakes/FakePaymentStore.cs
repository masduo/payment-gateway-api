using Features.Payments.Models;
using Features.Payments.Stores;

namespace Tests.Integration.Features.Payments.Fakes
{
    public class FakePaymentStore : IPaymentStoreWriter, IPaymentStoreReader
    {
        public const string FakeKeyThatExistsInStore = "00000000-1111-2222-3333-444444444444";

        public void Add(Payment payment)
        {
        }

        public Payment Retrieve(System.Guid id)
        {
            return id.ToString().Equals(FakeKeyThatExistsInStore)
                ? new Payment(
                    paymentRequest: new PaymentRequest { CardNumber = "1234-1234-1234-1234" },
                    bankResponse: new BankResponse(default, default))
                : null;
        }
    }
}
