using Features.Payments.Models;
using System;

namespace Features.Payments.Services
{
    public class BankService : IBankService
    {
        public BankResponse Process(PaymentRequest payment)
        {
            // simulate the bank response
            return new BankResponse(
                reference: Guid.NewGuid(),
                status: true);
        }
    }
}
