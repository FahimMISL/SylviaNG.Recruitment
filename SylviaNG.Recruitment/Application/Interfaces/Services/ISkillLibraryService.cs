using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface ISkillLibraryService
    {
        Task<List<SkillLibraryItemResponse>> GetAllAsync();
    }
}
