using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IAppointmentLetterService
    {
        Task<AppointmentLetterResponse> GenerateAsync(AppointmentLetterGenerateRequest request);
        Task<List<AppointmentLetterResponse>> GetAllAsync(long? jobApplicationId);
        Task<AppointmentLetterResponse> GetByIdAsync(long appointmentLetterId);
    }
}
