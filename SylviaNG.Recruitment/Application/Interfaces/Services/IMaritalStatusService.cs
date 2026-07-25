using SylviaNG.Recruitment.Application.Features.MaritalStatuses.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IMaritalStatusService
    {
        Task<long> CreateAsync(MaritalStatusCreateRequest request);
        Task UpdateAsync(long maritalStatusId, MaritalStatusUpdateRequest request);
        Task DeleteAsync(long maritalStatusId);
        Task<List<MaritalStatusResponse>> GetAllAsync();
    }
}
