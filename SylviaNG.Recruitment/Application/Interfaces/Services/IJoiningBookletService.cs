using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IJoiningBookletService
    {
        Task<List<JoiningBookletEligibleCandidateResponse>> GetEligibleCandidatesAsync();
        Task<JoiningBookletResponse> GenerateAsync(JoiningBookletGenerateRequest request);
        Task<JoiningBookletBulkGenerateResponse> BulkGenerateAsync(JoiningBookletBulkGenerateRequest request);
        Task<JoiningBookletFileResponse> BulkDownloadAsync(JoiningBookletBulkDownloadRequest request);
        Task<List<JoiningBookletResponse>> GetAllAsync(long? jobApplicationId);
        Task<JoiningBookletResponse> GetByIdAsync(long joiningBookletId);
    }
}
