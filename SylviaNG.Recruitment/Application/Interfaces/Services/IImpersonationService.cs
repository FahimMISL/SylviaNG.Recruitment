using SylviaNG.Recruitment.Application.Features.Impersonation.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IImpersonationService
    {
        /// <summary>SuperAdmin starts impersonating an Admin/HR UserAccount. Returns a short-lived
        /// (30 min) Local-scheme JWT carrying the target's own identity/role claims plus the
        /// session id - the frontend swaps its active token to this one for the session.</summary>
        Task<ImpersonationStartResponse> StartAsync(ImpersonationStartRequest request);

        /// <summary>Ends the session identified by the current request's own impersonation token
        /// (its "imp_sid" claim) - called while still using the impersonation token itself.</summary>
        Task EndCurrentAsync();
    }
}
