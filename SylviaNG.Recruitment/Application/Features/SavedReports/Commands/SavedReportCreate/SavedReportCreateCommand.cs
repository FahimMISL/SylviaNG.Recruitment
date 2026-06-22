using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedReports.Models;

namespace SylviaNG.Recruitment.Application.Features.SavedReports.Commands.SavedReportCreate
{
    public class SavedReportCreateCommand : IRequest<long>
    {
        public SavedReportCreateRequest Request { get; set; }

        public SavedReportCreateCommand(SavedReportCreateRequest request)
        {
            Request = request;
        }
    }
}
