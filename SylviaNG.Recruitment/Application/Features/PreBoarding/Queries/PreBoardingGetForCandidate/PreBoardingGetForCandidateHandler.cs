using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Queries.PreBoardingGetForCandidate
{
    public class PreBoardingGetForCandidateHandler : IRequestHandler<PreBoardingGetForCandidateQuery, PreBoardingSubmissionResponse>
    {
        private readonly IPreBoardingService _preBoardingService;

        public PreBoardingGetForCandidateHandler(IPreBoardingService preBoardingService)
        {
            _preBoardingService = preBoardingService;
        }

        public async Task<PreBoardingSubmissionResponse> Handle(PreBoardingGetForCandidateQuery query, CancellationToken cancellationToken)
        {
            return await _preBoardingService.GetForCurrentCandidateAsync();
        }
    }
}
