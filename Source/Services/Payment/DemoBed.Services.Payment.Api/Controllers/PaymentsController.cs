using DemoBed.Services.Payment.ApplicationService.Dtos;
using DemoBed.Services.Payment.ApplicationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DemoBed.Services.Payment.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentsController(IPaymentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPayments()
        {
            var list = await _service.GetPaymentsAsync();

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPayment(int id)
        {
            var payment = await _service.GetPaymentAsync(id);

            return Ok(payment);
        }

        [HttpPost]
        public async Task<IActionResult> PostPayment(
            [FromBody] UpsertPaymentDto payment)
        {
            var paymentResult = await _service.AddPaymentAsync(payment);

            return Ok(paymentResult);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutPayment(
            int id, UpsertPaymentDto payment)
        {
            var result = await _service.UpdatePaymentAsync(id, payment);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var result = await _service.DeletePaymentAsync(id);

            return Ok(result);
        }
    }
}
