using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Queries.ShortlistFilterCriteriaGetAllPaged
{
    public class ShortlistFilterCriteriaGetAllPagedHandler : IRequestHandler<ShortlistFilterCriteriaGetAllPagedQuery, PagedResult<ShortlistFilterCriteriaResponse>>
    {
        private readonly IShortlistFilterCriteriaService _shortlistFilterCriteriaService;

        public ShortlistFilterCriteriaGetAllPagedHandler(IShortlistFilterCriteriaService shortlistFilterCriteriaService)
        {
            _shortlistFilterCriteriaService = shortlistFilterCriteriaService;
        }

        public async Task<PagedResult<ShortlistFilterCriteriaResponse>> Handle(ShortlistFilterCriteriaGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _shortlistFilterCriteriaService.GetPaginatedAsync(query.Request);
        }
    }
}
