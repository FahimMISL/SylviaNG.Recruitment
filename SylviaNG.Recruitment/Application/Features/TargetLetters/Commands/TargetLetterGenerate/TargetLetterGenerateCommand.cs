using MediatR;
using SylviaNG.Recruitment.Application.Features.TargetLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.TargetLetters.Commands.TargetLetterGenerate
{
    public class TargetLetterGenerateCommand : IRequest<TargetLetterResponse>
    {
        public TargetLetterGenerateRequest Request { get; set; }

        public TargetLetterGenerateCommand(TargetLetterGenerateRequest request)
        {
            Request = request;
        }
    }
}
