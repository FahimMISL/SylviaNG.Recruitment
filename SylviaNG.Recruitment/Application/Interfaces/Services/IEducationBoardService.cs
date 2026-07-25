using SylviaNG.Recruitment.Application.Features.EducationBoards.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IEducationBoardService
    {
        Task<long> CreateAsync(EducationBoardCreateRequest request);
        Task UpdateAsync(long educationBoardId, EducationBoardUpdateRequest request);
        Task DeleteAsync(long educationBoardId);
        Task<List<EducationBoardResponse>> GetAllAsync();
    }
}
