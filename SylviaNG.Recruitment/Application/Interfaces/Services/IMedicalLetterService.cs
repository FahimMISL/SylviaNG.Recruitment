using SylviaNG.Recruitment.Application.Features.MedicalLetters.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IMedicalLetterService
    {
        Task<MedicalLetterResponse> GenerateAsync(MedicalLetterGenerateRequest request);
        Task<List<MedicalLetterResponse>> GetAllAsync(long? jobApplicationId);
        Task<MedicalLetterResponse> GetByIdAsync(long medicalLetterId);
    }
}
