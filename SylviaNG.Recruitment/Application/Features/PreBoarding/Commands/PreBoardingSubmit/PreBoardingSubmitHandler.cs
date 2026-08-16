using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingSubmit
{
    public class PreBoardingSubmitHandler : IRequestHandler<PreBoardingSubmitCommand, PreBoardingSubmissionResponse>
    {
        private readonly IPreBoardingService _preBoardingService;

        public PreBoardingSubmitHandler(IPreBoardingService preBoardingService)
        {
            _preBoardingService = preBoardingService;
        }

        public async Task<PreBoardingSubmissionResponse> Handle(PreBoardingSubmitCommand command, CancellationToken cancellationToken)
        {
            return await _preBoardingService.SubmitAsync();
        }
    }
}
