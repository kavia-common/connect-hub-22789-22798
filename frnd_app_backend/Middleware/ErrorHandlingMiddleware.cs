using System.Net;
using System.Text.Json;

namespace FrndAppBackend.Middleware
{
    /// Global exception handler returning consistent JSON problem details.
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            catch (UnauthorizedAccessException ex)
            {
                await WriteProblem(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                await WriteProblem(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                await WriteProblem(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteProblem(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteProblem(HttpContext ctx, HttpStatusCode status, string message)
        {
            ctx.Response.ContentType = "application/json";
            ctx.Response.StatusCode = (int)status;

            var body = new
            {
                error = new
                {
                    status = (int)status,
                    title = status.ToString(),
                    detail = message,
                    traceId = ctx.TraceIdentifier
                }
            };
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(body));
        }
    }
}
