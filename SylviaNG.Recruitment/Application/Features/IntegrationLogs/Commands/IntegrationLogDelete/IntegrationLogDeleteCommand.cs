using MediatR;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Commands.IntegrationLogDelete
{
    public class IntegrationLogDeleteCommand : IRequest<Unit>
    {
        public long IntegrationLogId { get; set; }

        public IntegrationLogDeleteCommand(long integrationLogId)
        {
            IntegrationLogId = integrationLogId;
        }
    }
}
