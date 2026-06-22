using MediatR;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestGetAllPaged
{
    public class ExportRequestGetAllPagedHandler : IRequestHandler<ExportRequestGetAllPagedQuery, PagedResult<ExportRequestResponse>>
    {
        private readonly IExportRequestService _exportRequestService;

        public ExportRequestGetAllPagedHandler(IExportRequestService exportRequestService)
        {
            _exportRequestService = exportRequestService;
        }

        public async Task<PagedResult<ExportRequestResponse>> Handle(ExportRequestGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _exportRequestService.GetPaginatedAsync(query.Request);
        }
    }
}
