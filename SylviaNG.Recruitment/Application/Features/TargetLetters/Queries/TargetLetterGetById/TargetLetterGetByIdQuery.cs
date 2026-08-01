using MediatR;
using SylviaNG.Recruitment.Application.Features.TargetLetters.Models;

namespace SylviaNG.Recruitment.Application.Features.TargetLetters.Queries.TargetLetterGetById
{
    public class TargetLetterGetByIdQuery : IRequest<TargetLetterResponse>
    {
        public long TargetLetterId { get; set; }

        public TargetLetterGetByIdQuery(long targetLetterId)
        {
            TargetLetterId = targetLetterId;
        }
    }
}
