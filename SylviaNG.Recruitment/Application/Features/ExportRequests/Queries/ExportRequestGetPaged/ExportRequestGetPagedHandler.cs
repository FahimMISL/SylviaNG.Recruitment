using MediatR;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Queries.ExportRequestGetPaged
{
    public class ExportRequestGetPagedHandler : IRequestHandler<ExportRequestGetPagedQuery, PagedResult<ExportRequestResponse>>
    {
        private readonly IExportRequestService _exportRequestService;

        public ExportRequestGetPagedHandler(IExportRequestService exportRequestService)
        {
            _exportRequestService = exportRequestService;
        }

        public async Task<PagedResult<ExportRequestResponse>> Handle(ExportRequestGetPagedQuery query, CancellationToken cancellationToken)
        {
            return await _exportRequestService.GetPagedAsync(query.Filter);
        }
    }
}
