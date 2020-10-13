using Features.Payments.Models;
using Features.Payments.Services;
using System;

namespace Tests.Integration.Features.Payments.Fakes
{
    /// <summary> Fakes the bank service methods, CardNumber ending wiht 9999 simulates a failed bank response </summary>
    public class FakeBankService : IBankService
    {
        public const string FailingCardNumber = "0000-1111-2222-3333";

        public BankResponse Process(PaymentRequest paymentRequest)
        {
            return new BankResponse(
                reference: default(Guid),
                status: !paymentRequest.CardNumber.Equals(FailingCardNumber));
        }
    }
}
