using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Commands.IntegrationLogCreate
{
    public class IntegrationLogCreateCommand : IRequest<long>
    {
        public IntegrationLogCreateRequest Request { get; set; }

        public IntegrationLogCreateCommand(IntegrationLogCreateRequest request)
        {
            Request = request;
        }
    }
}
