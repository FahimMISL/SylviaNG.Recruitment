using SylviaNG.Recruitment.Application.Features.TargetLetters.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ITargetLetterService
    {
        Task<TargetLetterResponse> GenerateAsync(TargetLetterGenerateRequest request);
        Task<List<TargetLetterResponse>> GetAllAsync(long? jobApplicationId);
        Task<TargetLetterResponse> GetByIdAsync(long targetLetterId);
    }
}
