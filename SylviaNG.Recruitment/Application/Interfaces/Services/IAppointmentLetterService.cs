using SylviaNG.Recruitment.Application.Features.AppointmentLetters.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IAppointmentLetterService
    {
        Task<AppointmentLetterResponse> GenerateAsync(AppointmentLetterGenerateRequest request);
        Task<List<AppointmentLetterResponse>> GetAllAsync(long? jobApplicationId);
        Task<AppointmentLetterResponse> GetByIdAsync(long appointmentLetterId);

        // Candidate self-service, mirrors OfferLetterService's *ForCandidate methods - identity
        // resolved internally via ICurrentCandidateService, ownership enforced in the handler.
        Task<List<AppointmentLetterResponse>> GetAllForCandidateAsync();
        Task<AppointmentLetterResponse> GetByIdForCandidateAsync(long appointmentLetterId);
    }
}
