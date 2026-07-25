using SylviaNG.Recruitment.Application.Features.OfferLetters.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IOfferLetterService
    {
        Task<OfferLetterResponse> GenerateAsync(OfferLetterGenerateRequest request);
        Task<List<OfferLetterResponse>> GetAllAsync(long? jobApplicationId);
        Task<OfferLetterResponse> GetByIdAsync(long offerLetterId);
    }
}
