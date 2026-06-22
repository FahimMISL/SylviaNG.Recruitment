using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedReports.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Queries.SavedReportGetById
{
    public class SavedReportGetByIdHandler : IRequestHandler<SavedReportGetByIdQuery, SavedReportResponse>
    {
        private readonly ISavedReportService _service;

        public SavedReportGetByIdHandler(ISavedReportService service)
        {
            _service = service;
        }

        public async Task<SavedReportResponse> Handle(SavedReportGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.SavedReportId);
        }
    }
}
