using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterApply
{
    public class ShortlistFilterApplyHandler : IRequestHandler<ShortlistFilterApplyCommand, ShortlistFilterApplyResponse>
    {
        private readonly IShortlistFilterEvaluationService _shortlistFilterEvaluationService;

        public ShortlistFilterApplyHandler(IShortlistFilterEvaluationService shortlistFilterEvaluationService)
        {
            _shortlistFilterEvaluationService = shortlistFilterEvaluationService;
        }

        public async Task<ShortlistFilterApplyResponse> Handle(ShortlistFilterApplyCommand command, CancellationToken cancellationToken)
        {
            return await _shortlistFilterEvaluationService.ApplyAsync(command.Request);
        }
    }
}
