using MediatR;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;

namespace SylviaNG.Recruitment.Application.Features.ExportRequests.Commands.ExportRequestCreate
{
    public class ExportRequestCreateCommand : IRequest<long>
    {
        public ExportRequestCreateRequest Request { get; set; }

        public ExportRequestCreateCommand(ExportRequestCreateRequest request)
        {
            Request = request;
        }
    }
}
