using MediatR;
using SylviaNG.Recruitment.Application.Features.TargetLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TargetLetters.Queries.TargetLetterGetById
{
    public class TargetLetterGetByIdHandler : IRequestHandler<TargetLetterGetByIdQuery, TargetLetterResponse>
    {
        private readonly ITargetLetterService _targetLetterService;

        public TargetLetterGetByIdHandler(ITargetLetterService targetLetterService)
        {
            _targetLetterService = targetLetterService;
        }

        public async Task<TargetLetterResponse> Handle(TargetLetterGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _targetLetterService.GetByIdAsync(query.TargetLetterId);
        }
    }
}
