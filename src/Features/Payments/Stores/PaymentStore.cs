using Features.Payments.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Features.Payments.Stores
{
    public class PaymentStore : IPaymentStoreReader, IPaymentStoreWriter
    {
        private static readonly ConcurrentDictionary<Guid, Payment> _payments;
        private readonly ILogger<PaymentStore> _logger;

        // static ctor to avoid undeterministic initialization
        static PaymentStore()
        {
            _payments = new ConcurrentDictionary<Guid, Payment>();
        }

        public PaymentStore(ILogger<PaymentStore> logger)
        {
            _logger = logger;
        }

        public void Add(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment), "Payment must not be null");

            if (!_payments.TryAdd(payment.Key, payment))
            {
                _logger.LogError("Failed to add {@payment}", payment);

                throw new InvalidOperationException($"Failed to add payment");
            }

            _logger.LogInformation("{@payment} added to store", payment);

            _logger.LogDebug("{keys}", _payments.Take(10).Select(p => $"\t\r\n{p.Key}"));
        }

        public Payment Retrieve(Guid key)
        {
            if (key == default)
                throw new ArgumentException(nameof(key), "Key must be set");

            _payments.TryGetValue(key, out var payment);

            return payment;
        }
    }
}
