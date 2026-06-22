using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedReports.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Queries.SavedReportGetAll
{
    public class SavedReportGetAllHandler : IRequestHandler<SavedReportGetAllQuery, List<SavedReportResponse>>
    {
        private readonly ISavedReportService _service;

        public SavedReportGetAllHandler(ISavedReportService service)
        {
            _service = service;
        }

        public async Task<List<SavedReportResponse>> Handle(SavedReportGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
