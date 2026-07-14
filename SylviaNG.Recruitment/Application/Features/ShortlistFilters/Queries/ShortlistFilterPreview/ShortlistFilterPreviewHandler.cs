using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterPreview
{
    public class ShortlistFilterPreviewHandler : IRequestHandler<ShortlistFilterPreviewQuery, ShortlistFilterPreviewResponse>
    {
        private readonly IShortlistFilterEvaluationService _shortlistFilterEvaluationService;

        public ShortlistFilterPreviewHandler(IShortlistFilterEvaluationService shortlistFilterEvaluationService)
        {
            _shortlistFilterEvaluationService = shortlistFilterEvaluationService;
        }

        public async Task<ShortlistFilterPreviewResponse> Handle(ShortlistFilterPreviewQuery query, CancellationToken cancellationToken)
        {
            return await _shortlistFilterEvaluationService.PreviewAsync(query.Request);
        }
    }
}
