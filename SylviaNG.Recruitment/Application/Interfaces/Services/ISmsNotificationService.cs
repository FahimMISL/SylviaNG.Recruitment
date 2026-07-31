namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ISmsNotificationService
    {
        Task<bool> TrySendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    }
}
