using SylviaNG.Recruitment.Application.Common.Exceptions;
using System.Text.Json;

namespace SylviaNG.Recruitment.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found.");
                await HandleExceptionAsync(context, StatusCodes.Status404NotFound, ex.Message);
            }
            catch (InvalidCredentialsException ex)
            {
                _logger.LogWarning(ex, "Invalid credentials.");
                await HandleExceptionAsync(context, StatusCodes.Status401Unauthorized, ex.Message);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate resource.");
                await HandleExceptionAsync(context, StatusCodes.Status409Conflict, ex.Message);
            }
            catch (InvalidStatusTransitionException ex)
            {
                _logger.LogWarning(ex, "Invalid status transition.");
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (ResourceInUseException ex)
            {
                _logger.LogWarning(ex, "Resource in use.");
                await HandleExceptionAsync(context, StatusCodes.Status409Conflict, ex.Message);
            }
            catch (KeycloakUnavailableException ex)
            {
                _logger.LogError(ex, "Keycloak unavailable.");
                await HandleExceptionAsync(context, StatusCodes.Status503ServiceUnavailable, "The authentication server is currently unavailable. Please try again later.");
            }
            catch (GroqUnavailableException ex)
            {
                _logger.LogError(ex, "Groq unavailable.");
                await HandleExceptionAsync(context, StatusCodes.Status503ServiceUnavailable, "The AI scoring service is currently unavailable. Please try again later.");
            }
            catch (SslCommerzUnavailableException ex)
            {
                _logger.LogError(ex, "SSLCommerz payment gateway unavailable.");
                await HandleExceptionAsync(context, StatusCodes.Status503ServiceUnavailable, "The payment gateway is currently unavailable. Please try again later.");
            }
            catch (FluentValidation.ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed.");
                var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();

                var response = new
                {
                    hasError = true,
                    decentMessage = "Validation failed.",
                    errorDetails = errors,
                    content = (object?)null
                };

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please contact support.");
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                hasError = true,
                decentMessage = message,
                errorDetails = (string?)null,
                content = (object?)null
            };

            var json = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(json);
        }
    }
}
