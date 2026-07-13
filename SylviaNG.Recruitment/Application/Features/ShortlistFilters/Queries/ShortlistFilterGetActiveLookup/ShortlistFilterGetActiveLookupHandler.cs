using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetActiveLookup
{
    public class ShortlistFilterGetActiveLookupHandler : IRequestHandler<ShortlistFilterGetActiveLookupQuery, List<ShortlistFilterLookupResponse>>
    {
        private readonly IShortlistFilterService _shortlistFilterService;

        public ShortlistFilterGetActiveLookupHandler(IShortlistFilterService shortlistFilterService)
        {
            _shortlistFilterService = shortlistFilterService;
        }

        public async Task<List<ShortlistFilterLookupResponse>> Handle(ShortlistFilterGetActiveLookupQuery query, CancellationToken cancellationToken)
        {
            return await _shortlistFilterService.GetActiveLookupAsync();
        }
    }
}
