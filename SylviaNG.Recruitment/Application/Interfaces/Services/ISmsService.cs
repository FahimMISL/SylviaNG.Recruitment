namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ISmsService
    {
        Task<bool> SendAsync(string phoneNumber, string message);
        Task<List<(string PhoneNumber, bool Success)>> SendBulkAsync(List<string> phoneNumbers, string message);
    }
}
