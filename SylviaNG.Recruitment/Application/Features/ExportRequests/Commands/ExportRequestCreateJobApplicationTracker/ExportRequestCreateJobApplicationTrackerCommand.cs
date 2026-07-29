using MediatR;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestCreateJobApplicationTracker
{
    public class ExportRequestCreateJobApplicationTrackerCommand : IRequest<long>
    {
        public ExportRequestCreateRequest Request { get; set; }

        public ExportRequestCreateJobApplicationTrackerCommand(ExportRequestCreateRequest request)
        {
            Request = request;
        }
    }
}
