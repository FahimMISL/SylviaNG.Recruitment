using MediatR;
using SylviaNG.Recruitment.Application.Features.TargetLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TargetLetters.Commands.TargetLetterGenerate
{
    public class TargetLetterGenerateHandler : IRequestHandler<TargetLetterGenerateCommand, TargetLetterResponse>
    {
        private readonly ITargetLetterService _targetLetterService;

        public TargetLetterGenerateHandler(ITargetLetterService targetLetterService)
        {
            _targetLetterService = targetLetterService;
        }

        public async Task<TargetLetterResponse> Handle(TargetLetterGenerateCommand command, CancellationToken cancellationToken)
        {
            return await _targetLetterService.GenerateAsync(command.Request);
        }
    }
}
