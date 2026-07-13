using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetAll
{
    public class ShortlistFilterGetAllHandler : IRequestHandler<ShortlistFilterGetAllQuery, List<ShortlistFilterResponse>>
    {
        private readonly IShortlistFilterService _shortlistFilterService;

        public ShortlistFilterGetAllHandler(IShortlistFilterService shortlistFilterService)
        {
            _shortlistFilterService = shortlistFilterService;
        }

        public async Task<List<ShortlistFilterResponse>> Handle(ShortlistFilterGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _shortlistFilterService.GetAllAsync();
        }
    }
}
