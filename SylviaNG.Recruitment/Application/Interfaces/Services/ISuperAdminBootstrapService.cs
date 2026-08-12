namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    /// <summary>
    /// One-time bootstrap for the very first SuperAdmin account. Runs on every startup but is a
    /// no-op once any SuperAdmin UserAccount exists, so it's safe to leave wired in permanently -
    /// see SuperAdminBootstrapService for the full explanation.
    /// </summary>
    public interface ISuperAdminBootstrapService
    {
        Task RunAsync();
    }
}
