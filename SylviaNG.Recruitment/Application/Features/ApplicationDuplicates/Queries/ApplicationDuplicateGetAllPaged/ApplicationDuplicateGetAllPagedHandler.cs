using MediatR;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Queries.ApplicationDuplicateGetAllPaged
{
    public class ApplicationDuplicateGetAllPagedHandler : IRequestHandler<ApplicationDuplicateGetAllPagedQuery, PagedResult<ApplicationDuplicateResponse>>
    {
        private readonly IApplicationDuplicateService _applicationDuplicateService;

        public ApplicationDuplicateGetAllPagedHandler(IApplicationDuplicateService applicationDuplicateService)
        {
            _applicationDuplicateService = applicationDuplicateService;
        }

        public async Task<PagedResult<ApplicationDuplicateResponse>> Handle(ApplicationDuplicateGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _applicationDuplicateService.GetPaginatedAsync(query.Request);
        }
    }
}
