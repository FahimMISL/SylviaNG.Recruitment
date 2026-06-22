using MediatR;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Commands.SavedReportDelete
{
    public class SavedReportDeleteCommand : IRequest<Unit>
    {
        public long SavedReportId { get; set; }

        public SavedReportDeleteCommand(long savedReportId)
        {
            SavedReportId = savedReportId;
        }
    }
}
