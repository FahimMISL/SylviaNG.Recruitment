using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IPreBoardingSubmissionRepository : IRepository<PreBoardingSubmission>
    {
        Task<PreBoardingSubmission?> GetByFinalSelectionPoolIdWithDetailsAsync(long finalSelectionPoolId);
    }
}
