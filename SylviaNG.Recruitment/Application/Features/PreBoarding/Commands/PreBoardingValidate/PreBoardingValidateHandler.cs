using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingValidate
{
    public class PreBoardingValidateHandler : IRequestHandler<PreBoardingValidateCommand, PreBoardingSubmissionResponse>
    {
        private readonly IPreBoardingService _preBoardingService;

        public PreBoardingValidateHandler(IPreBoardingService preBoardingService)
        {
            _preBoardingService = preBoardingService;
        }

        public async Task<PreBoardingSubmissionResponse> Handle(PreBoardingValidateCommand command, CancellationToken cancellationToken)
        {
            return await _preBoardingService.ValidateAsync(command.PreBoardingSubmissionId);
        }
    }
}
