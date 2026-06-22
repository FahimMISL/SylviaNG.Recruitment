using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IEmergencyContactService
    {
        Task<long> CreateAsync(EmergencyContactCreateRequest request);
        Task UpdateAsync(long emergencyContactId, EmergencyContactUpdateRequest request);
        Task DeleteAsync(long emergencyContactId);
        Task<List<EmergencyContactResponse>> GetAllAsync();
        Task<EmergencyContactResponse> GetByIdAsync(long emergencyContactId);
        Task<PagedResult<EmergencyContactResponse>> GetPaginatedAsync(PagedRequest request);
    }
}
