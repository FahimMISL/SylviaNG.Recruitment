using SylviaNG.Recruitment.Application.Features.BloodGroups.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IBloodGroupService
    {
        Task<long> CreateAsync(BloodGroupCreateRequest request);
        Task UpdateAsync(long bloodGroupId, BloodGroupUpdateRequest request);
        Task DeleteAsync(long bloodGroupId);
        Task<List<BloodGroupResponse>> GetAllAsync();
    }
}
