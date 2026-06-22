using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ApplicationScreeningResults.Queries.ApplicationScreeningResultGetAllPaged
{
    public class ApplicationScreeningResultGetAllPagedHandler : IRequestHandler<ApplicationScreeningResultGetAllPagedQuery, PagedResult<ApplicationScreeningResultResponse>>
    {
        private readonly IApplicationScreeningResultService _applicationScreeningResultService;

        public ApplicationScreeningResultGetAllPagedHandler(IApplicationScreeningResultService applicationScreeningResultService)
        {
            _applicationScreeningResultService = applicationScreeningResultService;
        }

        public async Task<PagedResult<ApplicationScreeningResultResponse>> Handle(ApplicationScreeningResultGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _applicationScreeningResultService.GetPaginatedAsync(query.Request);
        }
    }
}
