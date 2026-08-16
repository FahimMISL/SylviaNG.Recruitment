using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Queries.PreBoardingIsEligible
{
    public class PreBoardingIsEligibleHandler : IRequestHandler<PreBoardingIsEligibleQuery, bool>
    {
        private readonly IPreBoardingService _preBoardingService;

        public PreBoardingIsEligibleHandler(IPreBoardingService preBoardingService)
        {
            _preBoardingService = preBoardingService;
        }

        public async Task<bool> Handle(PreBoardingIsEligibleQuery query, CancellationToken cancellationToken)
        {
            return await _preBoardingService.IsEligibleForCurrentCandidateAsync();
        }
    }
}
