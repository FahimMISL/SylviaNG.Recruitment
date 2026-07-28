using MediatR;
using SylviaNG.Recruitment.Application.Features.TargetLetters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.TargetLetters.Queries.TargetLetterGetAll
{
    public class TargetLetterGetAllHandler : IRequestHandler<TargetLetterGetAllQuery, List<TargetLetterResponse>>
    {
        private readonly ITargetLetterService _targetLetterService;

        public TargetLetterGetAllHandler(ITargetLetterService targetLetterService)
        {
            _targetLetterService = targetLetterService;
        }

        public async Task<List<TargetLetterResponse>> Handle(TargetLetterGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _targetLetterService.GetAllAsync(query.JobApplicationId);
        }
    }
}
