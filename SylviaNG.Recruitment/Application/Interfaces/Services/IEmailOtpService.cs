namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IEmailOtpService
    {
        Task<(bool Success, string Message)> SendOtpAsync(string email);
        Task<(bool Success, string Message)> VerifyOtpAsync(string email, string otpCode);
    }
}
