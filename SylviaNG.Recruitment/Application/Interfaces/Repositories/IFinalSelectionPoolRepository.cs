using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IFinalSelectionPoolRepository : IRepository<FinalSelectionPool>
    {
        Task<List<FinalSelectionPool>> GetAllOrderedAsync();
        Task<FinalSelectionPool?> GetByIdWithDetailsAsync(long finalSelectionPoolId);
        Task<FinalSelectionPool?> GetByOfferLetterIdAsync(long offerLetterId);
        Task<FinalSelectionPool?> GetByCandidateProfileIdWithDetailsAsync(long candidateProfileId);
    }
}
