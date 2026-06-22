using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedReports.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Queries.SavedReportGetAllPaged
{
    public class SavedReportGetAllPagedHandler : IRequestHandler<SavedReportGetAllPagedQuery, PagedResult<SavedReportResponse>>
    {
        private readonly ISavedReportService _savedReportService;

        public SavedReportGetAllPagedHandler(ISavedReportService savedReportService)
        {
            _savedReportService = savedReportService;
        }

        public async Task<PagedResult<SavedReportResponse>> Handle(SavedReportGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _savedReportService.GetPaginatedAsync(query.Request);
        }
    }
}
