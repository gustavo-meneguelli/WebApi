namespace Api.Middlewares;

public class GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu um erro não tratado.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var errorResponse = new
        {
            Status = context.Response.StatusCode,
            Message = "Ocorreu um erro interno no servidor. Tente novamente mais tarde.",
            Detailed = exception.Message
        };

        var json = System.Text.Json.JsonSerializer.Serialize(errorResponse);

        await context.Response.WriteAsync(json);
    }
}