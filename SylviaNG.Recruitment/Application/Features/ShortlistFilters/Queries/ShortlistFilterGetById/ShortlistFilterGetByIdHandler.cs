using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetById
{
    public class ShortlistFilterGetByIdHandler : IRequestHandler<ShortlistFilterGetByIdQuery, ShortlistFilterResponse>
    {
        private readonly IShortlistFilterService _shortlistFilterService;

        public ShortlistFilterGetByIdHandler(IShortlistFilterService shortlistFilterService)
        {
            _shortlistFilterService = shortlistFilterService;
        }

        public async Task<ShortlistFilterResponse> Handle(ShortlistFilterGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _shortlistFilterService.GetByIdAsync(query.ShortlistFilterId);
        }
    }
}
