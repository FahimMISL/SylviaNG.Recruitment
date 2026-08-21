using SylviaNG.Recruitment.Application.Common.Authorization;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using System.Text.Json;

namespace SylviaNG.Recruitment.Middlewares
{
    /// <summary>
    /// EP-15/US-115: on every authenticated request carrying an impersonation token (identified
    /// by the "imp_sid" claim - see ImpersonationClaimTypes/ImpersonationService), re-validates
    /// the session live against the database (not just the JWT's own expiry, so an explicit
    /// EndAsync takes effect immediately instead of waiting out the token) and appends an
    /// ImpersonationLog row. Runs after UseAuthentication so claims are populated.
    /// </summary>
    public class ImpersonationMiddleware
    {
        private readonly RequestDelegate _next;

        public ImpersonationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IImpersonationSessionRepository impersonationSessionRepository, IUnitOfWork unitOfWork)
        {
            var sessionIdClaim = context.User.FindFirst(ImpersonationClaimTypes.SessionId)?.Value;
            if (string.IsNullOrEmpty(sessionIdClaim) || !long.TryParse(sessionIdClaim, out var sessionId))
            {
                await _next(context);
                return;
            }

            var session = await impersonationSessionRepository.GetByIdAsync(sessionId);
            var nowUtc = DateTime.UtcNow;

            if (session is null || session.EndedAt is not null || session.ExpiresAt <= nowUtc)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var body = JsonSerializer.Serialize(new
                {
                    hasError = true,
                    decentMessage = "This impersonation session has ended or expired.",
                    errorDetails = (string?)null,
                    content = (object?)null
                });
                await context.Response.WriteAsync(body);
                return;
            }

            unitOfWork.Context.ImpersonationLogs.Add(new ImpersonationLog
            {
                ImpersonationSessionId = sessionId,
                HttpMethod = context.Request.Method,
                Path = context.Request.Path.ToString(),
                Timestamp = nowUtc
            });
            await unitOfWork.SaveChangesAsync();

            await _next(context);
        }
    }
}
