using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); // Chama o próximo middleware/controller
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro capturado pelo middleware: {ex.Message}");
                await HandleExceptionAsync(httpContext, ex); // Lida com a exceção e responde ao cliente
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                InvalidOperationException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError, // Erro genérico
            };

            var result = new
            {
                mensagem = exception.Message,
                statusCode = context.Response.StatusCode
            };

            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(result));
        }
    }
}
