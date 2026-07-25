using System.Text.Json;

namespace SylviaNG.Recruitment.Middlewares
{
    public class ResponseWrappingMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseWrappingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                // Handle 204 as wrapped 200 with null content
                if (context.Response.StatusCode == StatusCodes.Status204NoContent)
                {
                    context.Response.Body = originalBodyStream;
                    context.Response.StatusCode = StatusCodes.Status200OK;

                    var wrappedEmptyResponse = new
                    {
                        hasError = false,
                        decentMessage = "No content, but request processed successfully.",
                        errorDetails = (string?)null,
                        content = (object?)null
                    };

                    var json = JsonSerializer.Serialize(wrappedEmptyResponse);
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(json);
                    return;
                }

                // Check if response has already started (headers sent) - don't wrap if so
                if (context.Response.HasStarted)
                {
                    context.Response.Body = originalBodyStream;
                    return;
                }

                // Binary/file responses (e.g. controller File() results, identified by a
                // Content-Disposition header or a non-JSON/non-text Content-Type) must pass
                // through untouched - reading them as UTF-8 text and re-serializing as JSON
                // corrupts the bytes.
                var contentType = context.Response.ContentType;
                var isBinaryOrFileResponse = context.Response.Headers.ContainsKey("Content-Disposition")
                    || (contentType != null
                        && !contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase)
                        && !contentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase));

                if (isBinaryOrFileResponse)
                {
                    context.Response.Body = originalBodyStream;
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                    return;
                }

                responseBody.Seek(0, SeekOrigin.Begin);
                var bodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                if (context.Response.StatusCode >= 400)
                {
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                    return;
                }

                object? responseContent;
                try
                {
                    responseContent = JsonSerializer.Deserialize<object>(bodyText, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch
                {
                    responseContent = bodyText;
                }

                var wrappedResponse = new
                {
                    hasError = false,
                    decentMessage = "Request processed successfully.",
                    errorDetails = (string?)null,
                    content = responseContent
                };

                var wrappedJson = JsonSerializer.Serialize(wrappedResponse);

                context.Response.Body = originalBodyStream;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(wrappedJson);
            }
            catch
            {
                // Restore original stream and re-throw
                context.Response.Body = originalBodyStream;
                throw;
            }
        }
    }
}
