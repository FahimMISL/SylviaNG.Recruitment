using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Queries.PreBoardingGetByPoolForHr
{
    public class PreBoardingGetByPoolForHrHandler : IRequestHandler<PreBoardingGetByPoolForHrQuery, PreBoardingSubmissionResponse>
    {
        private readonly IPreBoardingService _preBoardingService;

        public PreBoardingGetByPoolForHrHandler(IPreBoardingService preBoardingService)
        {
            _preBoardingService = preBoardingService;
        }

        public async Task<PreBoardingSubmissionResponse> Handle(PreBoardingGetByPoolForHrQuery query, CancellationToken cancellationToken)
        {
            return await _preBoardingService.GetByFinalSelectionPoolIdForHrAsync(query.FinalSelectionPoolId);
        }
    }
}
