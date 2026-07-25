using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IOfferLetterRepository : IRepository<OfferLetter>
    {
        Task<List<OfferLetter>> GetAllOrderedAsync(long? jobApplicationId);
        Task<OfferLetter?> GetByIdWithDetailsAsync(long offerLetterId);
        Task<List<OfferLetter>> GetAllForCandidateAsync(long candidateProfileId);

        // EP-10 US-084: candidates eligible for a joining-booklet batch (batch stub - no
        // FinalSelectionPool entity exists yet, see JoiningBooklet.cs).
        Task<List<OfferLetter>> GetAcceptedOrderedAsync();
        Task<List<OfferLetter>> GetByIdsWithDetailsAsync(List<long> offerLetterIds);
    }
}
