using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Queries.IntegrationLogGetById
{
    public class IntegrationLogGetByIdQuery : IRequest<IntegrationLogResponse>
    {
        public long IntegrationLogId { get; set; }

        public IntegrationLogGetByIdQuery(long integrationLogId)
        {
            IntegrationLogId = integrationLogId;
        }
    }
}
