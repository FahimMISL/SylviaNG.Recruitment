using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Queries.IntegrationConfigGetAll
{
    public class IntegrationConfigGetAllQuery : IRequest<List<IntegrationConfigResponse>>
    {
    }
}
