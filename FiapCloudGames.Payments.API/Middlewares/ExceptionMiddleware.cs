using FiapCloudGames.Payments.Domain.Exceptions;
using Serilog;

namespace FiapCloudGames.Payments.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = ex switch
            {
                PaymentNotFoundException => StatusCodes.Status404NotFound,
                PaymentFraudDetectedException => StatusCodes.Status422UnprocessableEntity,
                InvalidFormException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            if (context.Response.StatusCode == StatusCodes.Status500InternalServerError)
                Log.Error(ex, "Erro interno no serviço FiapCloudGames.Payments");
        }
    }
}
