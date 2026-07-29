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

        /// <summary>EP-14 US-105 AC1: offers generated/sent but not yet accepted/declined - the
        /// "Offers Pending Acceptance" dashboard metric. Mutable point-in-time state (no history
        /// table), so this has no trend delta - see DashboardService for which metrics do.</summary>
        Task<int> CountPendingAcceptanceAsync();
    }
}
