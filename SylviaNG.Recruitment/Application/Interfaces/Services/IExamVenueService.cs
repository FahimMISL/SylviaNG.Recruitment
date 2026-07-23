using SylviaNG.Recruitment.Application.Features.ExamVenues.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IExamVenueService
    {
        Task<long> CreateAsync(ExamVenueCreateRequest request);
        Task UpdateAsync(long examVenueId, ExamVenueUpdateRequest request);
        Task SetActiveStatusAsync(long examVenueId, bool isActive);
        Task<ExamVenueResponse> GetByIdAsync(long examVenueId);
        Task<List<ExamVenueResponse>> GetAllAsync();
        Task<List<ExamVenueLookupResponse>> GetActiveLookupAsync();
    }
}
