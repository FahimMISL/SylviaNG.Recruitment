using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedReports.Models;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Commands.SavedReportUpdate
{
    public class SavedReportUpdateCommand : IRequest<Unit>
    {
        public long SavedReportId { get; set; }
        public SavedReportUpdateRequest Request { get; set; }

        public SavedReportUpdateCommand(long savedReportId, SavedReportUpdateRequest request)
        {
            SavedReportId = savedReportId;
            Request = request;
        }
    }
}
