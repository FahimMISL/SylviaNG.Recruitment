using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IPreBoardingService
    {
        Task<PreBoardingSubmissionResponse> GetForCurrentCandidateAsync();

        /// <summary>True once the current candidate has a Final Selection Pool entry (i.e. they've
        /// accepted an offer) - drives whether the frontend shows the Pre-Boarding nav item at all,
        /// instead of every candidate seeing it and hitting a NotFoundException.</summary>
        Task<bool> IsEligibleForCurrentCandidateAsync();
        Task<PreBoardingSubmissionResponse> SaveDraftAsync(PreBoardingSaveRequest request);
        Task<PreBoardingSubmissionResponse> SubmitAsync();

        // EP-12 US-096: HR validate/lock + correction-request workflow.
        Task<PreBoardingSubmissionResponse> GetByFinalSelectionPoolIdForHrAsync(long finalSelectionPoolId);
        Task<PreBoardingSubmissionResponse> ValidateAsync(long preBoardingSubmissionId);
        Task<PreBoardingSubmissionResponse> RequestCorrectionAsync(long preBoardingSubmissionId, string comment);
    }
}
