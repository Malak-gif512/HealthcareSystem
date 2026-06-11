using System.Net;
using System.Text.Json;
using HealthcareSystem.Application.DTOs;

namespace HealthcareSystem.Api.Middlewares
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Proceed to the next middleware/controller
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Map specific exception messages to appropriate HTTP Status Codes
            if (exception.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound; // Returns 404
            }
            else if (exception.Message.Contains("already exists") || exception.Message.Contains("conflict"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict; // Returns 409
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Returns 500
            }

            var response = ApiResponse<string>.Fail(exception.Message);
            var json = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(json);
        }
    }
}