using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingRequestCorrection
{
    public class PreBoardingRequestCorrectionHandler : IRequestHandler<PreBoardingRequestCorrectionCommand, PreBoardingSubmissionResponse>
    {
        private readonly IPreBoardingService _preBoardingService;

        public PreBoardingRequestCorrectionHandler(IPreBoardingService preBoardingService)
        {
            _preBoardingService = preBoardingService;
        }

        public async Task<PreBoardingSubmissionResponse> Handle(PreBoardingRequestCorrectionCommand command, CancellationToken cancellationToken)
        {
            return await _preBoardingService.RequestCorrectionAsync(command.PreBoardingSubmissionId, command.Request.Comment);
        }
    }
}
