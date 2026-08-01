using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IFinalSelectionPoolService
    {
        /// <summary>EP-12 US-094: internal only, called from OfferLetterService.AcceptAsync right
        /// after an offer is accepted - never exposed via its own Command/HTTP endpoint.</summary>
        Task CreateFromAcceptedOfferAsync(OfferLetter offerLetter);

        Task<List<FinalSelectionPoolResponse>> GetAllAsync();
        Task<FinalSelectionPoolResponse> GetByIdAsync(long finalSelectionPoolId);
        Task<FinalSelectionPoolResponse> MarkHasJoinedAsync(long finalSelectionPoolId);
        Task<FinalSelectionPoolResponse> UpdateBatchAsync(long finalSelectionPoolId, FinalSelectionPoolUpdateBatchRequest request);
    }
}
