using Domain.Constants;

namespace Api.Middlewares;

public class GlobalErrorHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalErrorHandlerMiddleware> logger,
    IWebHostEnvironment environment)
{
    // Captura todas as exceções não tratadas da aplicação
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ErrorMessages.UnhandledError);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Detalhes da exception expostos apenas em Development
        var errorResponse = new
        {
            Status = context.Response.StatusCode,
            Message = ErrorMessages.InternalServerError,
            Detailed = environment.IsDevelopment() ? exception.Message : null
        };

        var json = System.Text.Json.JsonSerializer.Serialize(errorResponse);

        await context.Response.WriteAsync(json);
    }
}
