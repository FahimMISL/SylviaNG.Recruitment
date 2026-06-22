using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetAllPaged
{
    public class ShortlistFilterGetAllPagedHandler : IRequestHandler<ShortlistFilterGetAllPagedQuery, PagedResult<ShortlistFilterResponse>>
    {
        private readonly IShortlistFilterService _shortlistFilterService;

        public ShortlistFilterGetAllPagedHandler(IShortlistFilterService shortlistFilterService)
        {
            _shortlistFilterService = shortlistFilterService;
        }

        public async Task<PagedResult<ShortlistFilterResponse>> Handle(ShortlistFilterGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _shortlistFilterService.GetPaginatedAsync(query.Request);
        }
    }
}
