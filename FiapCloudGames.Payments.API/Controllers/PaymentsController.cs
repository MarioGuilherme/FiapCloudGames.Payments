using FiapCloudGames.Payments.API.Extensions;
using FiapCloudGames.Payments.Application.InputModels;
using FiapCloudGames.Payments.Application.Interfaces;
using FiapCloudGames.Payments.Application.ViewModels;
using FiapCloudGames.Payments.Domain.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Serilog;

namespace FiapCloudGames.Payments.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;

    [HttpGet]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllMyPayments()
    {
        RestResponse<IEnumerable<PaymentViewModel>> restResponse = await _paymentService.GetAllByUserIdAsync(User.UserId());
        return Ok(restResponse);
    }

    [HttpGet("{paymentId}")]
    [Authorize]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int paymentId)
    {
        RestResponse<PaymentViewModel> restResponse = await _paymentService.GetByIdAsync(paymentId);
        return Ok(restResponse);
    }

    [HttpPost("receivePaymentWebhookFunction")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ReceivePaymentWebhookFunction(ReceivedPaymentEvent receivedPaymentEvent, [FromHeader(Name = "X-key-webhook")] string keyPagSeguro)
    {
        if (keyPagSeguro != "minhaChaveSecretaDoPagSeguro") // Aqui garanto que só recebo request da PagSeguro, por ser WebHook
        {
            Log.Warning("Tentativa de acesso ao Webhook receivePaymentWebhookFunction inválida pela X-key-webhook");
            return NotFound();
        }
        await _paymentService.ProcessWebHookAsync(receivedPaymentEvent);
        return NoContent();
    }
}
