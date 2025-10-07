using FiapCloudGames.Payments.API.Extensions;
using FiapCloudGames.Payments.Application.InputModels;
using FiapCloudGames.Payments.Application.Interfaces;
using FiapCloudGames.Payments.Application.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<CreatedAtActionResult> Create([FromBody] CreatePaymentInputModel inputModel)
    {
        RestResponse<PaymentViewModel> restResponse = await _paymentService.CreateAsync(inputModel);
        return CreatedAtAction(nameof(GetById), new { paymentId = restResponse.Data!.PaymentId }, restResponse);
    }

    [HttpPatch("{paymentId}/mark-as-paid")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<NoContentResult> MarkAsPaid(int paymentId)
    {
        await _paymentService.MarkAsPaidAsync(paymentId);
        return NoContent();
    }

    [HttpPatch("{paymentId}/cancel")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<NoContentResult> Cancel(int paymentId)
    {
        await _paymentService.CancelByIdAsync(paymentId);
        return NoContent();
    }
}
