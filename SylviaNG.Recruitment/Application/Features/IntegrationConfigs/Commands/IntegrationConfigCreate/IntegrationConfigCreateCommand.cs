using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Commands.IntegrationConfigCreate
{
    public class IntegrationConfigCreateCommand : IRequest<long>
    {
        public IntegrationConfigCreateRequest Request { get; set; }

        public IntegrationConfigCreateCommand(IntegrationConfigCreateRequest request)
        {
            Request = request;
        }
    }
}
