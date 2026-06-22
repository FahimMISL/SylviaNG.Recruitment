using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Commands.IntegrationLogUpdate
{
    public class IntegrationLogUpdateCommand : IRequest<Unit>
    {
        public long IntegrationLogId { get; set; }
        public IntegrationLogUpdateRequest Request { get; set; }

        public IntegrationLogUpdateCommand(long integrationLogId, IntegrationLogUpdateRequest request)
        {
            IntegrationLogId = integrationLogId;
            Request = request;
        }
    }
}
