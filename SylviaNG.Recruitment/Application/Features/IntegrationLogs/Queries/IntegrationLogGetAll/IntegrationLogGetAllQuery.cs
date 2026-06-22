using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Queries.IntegrationLogGetAll
{
    public class IntegrationLogGetAllQuery : IRequest<List<IntegrationLogResponse>>
    {
    }
}
