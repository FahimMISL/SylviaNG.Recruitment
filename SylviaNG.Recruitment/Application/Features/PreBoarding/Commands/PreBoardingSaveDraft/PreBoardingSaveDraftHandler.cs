using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingSaveDraft
{
    public class PreBoardingSaveDraftHandler : IRequestHandler<PreBoardingSaveDraftCommand, PreBoardingSubmissionResponse>
    {
        private readonly IPreBoardingService _preBoardingService;

        public PreBoardingSaveDraftHandler(IPreBoardingService preBoardingService)
        {
            _preBoardingService = preBoardingService;
        }

        public async Task<PreBoardingSubmissionResponse> Handle(PreBoardingSaveDraftCommand command, CancellationToken cancellationToken)
        {
            return await _preBoardingService.SaveDraftAsync(command.Request);
        }
    }
}
