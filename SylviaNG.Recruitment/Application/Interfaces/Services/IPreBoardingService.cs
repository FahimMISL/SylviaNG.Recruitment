using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;

namespace SylviaNG.Recruitment.Application.Interfaces.Services
{
    public interface IPreBoardingService
    {
        Task<PreBoardingSubmissionResponse> GetForCurrentCandidateAsync();
        Task<PreBoardingSubmissionResponse> SaveDraftAsync(PreBoardingSaveRequest request);
        Task<PreBoardingSubmissionResponse> SubmitAsync();
    }
}
