using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedReports.Models;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Queries.SavedReportGetById
{
    public class SavedReportGetByIdQuery : IRequest<SavedReportResponse>
    {
        public long SavedReportId { get; set; }

        public SavedReportGetByIdQuery(long savedReportId)
        {
            SavedReportId = savedReportId;
        }
    }
}
