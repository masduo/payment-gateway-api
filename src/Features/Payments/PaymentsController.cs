using Features.Payments.Models;
using Features.Payments.Services;
using Features.Payments.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.Api;
using System;
using System.Net.Mime;

namespace Features.Payments
{
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly ILogger<PaymentsController> _logger;
        private readonly IPaymentStoreWriter _paymentStoreWriter;
        private readonly IPaymentStoreReader _paymentStoreReader;
        private readonly IBankService _bankService;

        public PaymentsController(
            ILogger<PaymentsController> logger,
            IBankService bankService,
            IPaymentStoreWriter paymentStoreWriter,
            IPaymentStoreReader paymentStoreReader)
        {
            _logger = logger;
            _bankService = bankService;
            _paymentStoreWriter = paymentStoreWriter;
            _paymentStoreReader = paymentStoreReader;
        }

        /// <summary> Processes payment requests </summary>
        /// <param name="paymentRequest"> Payment request parameters </param>
        /// <response code="400"> `BadRequest` when request is invalid </response>
        [HttpPost]
        [ApiVersion(Startup.DefaultApiVersion)]
        [Route("/v{version:apiVersion}/payments")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
        public IActionResult Process([FromBody] PaymentRequest paymentRequest)
        {
            if (paymentRequest.ExpiryYear == DateTime.UtcNow.Year &&
                paymentRequest.ExpiryMonth < DateTime.UtcNow.Month)
            {
                ModelState.AddModelError(nameof(PaymentRequest.ExpiryMonth), "Expiry date is in the past");

                return BadRequest(ModelState);
            }

            var bankResponse = _bankService.Process(paymentRequest);

            var payment = new Payment(paymentRequest, bankResponse);

            _paymentStoreWriter.Add(payment);

            return Ok(new PaymentResponse { Payment = payment });
        }

        /// <summary> Retrieves details of a previously made payment request </summary>
        /// <param name="key" example="3fa85f64-5717-4562-b3fc-2c963f66afa6"> Payment request parameters </param>
        /// <response code="400"> `BadRequest` when key is not set </response>
        /// <response code="404"> `NotFound` when no payment found using key </response>
        [HttpGet]
        [ApiVersion(Startup.DefaultApiVersion)]
        [Route("/v{version:apiVersion}/payments/{key}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        public IActionResult Retrieve([FromRoute] Guid key)
        {
            if (key == default)
                return BadRequest("Key must be set");

            var payment = _paymentStoreReader.Retrieve(key);

            if (payment == null)
                return NotFound($"No payment found using key: `{key}");

            return Ok(new PaymentResponse { Payment = payment });
        }
    }
}
