using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IPreBoardingService
    {
        Task<PreBoardingSubmissionResponse> GetForCurrentCandidateAsync();
        Task<PreBoardingSubmissionResponse> SaveDraftAsync(PreBoardingSaveRequest request);
        Task<PreBoardingSubmissionResponse> SubmitAsync();

        // EP-12 US-096: HR validate/lock + correction-request workflow.
        Task<PreBoardingSubmissionResponse> GetByFinalSelectionPoolIdForHrAsync(long finalSelectionPoolId);
        Task<PreBoardingSubmissionResponse> ValidateAsync(long preBoardingSubmissionId);
        Task<PreBoardingSubmissionResponse> RequestCorrectionAsync(long preBoardingSubmissionId, string comment);
    }
}
