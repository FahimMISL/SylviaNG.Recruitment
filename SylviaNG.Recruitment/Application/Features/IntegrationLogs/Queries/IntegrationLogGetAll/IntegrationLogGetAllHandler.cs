using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Queries.IntegrationLogGetAll
{
    public class IntegrationLogGetAllHandler : IRequestHandler<IntegrationLogGetAllQuery, List<IntegrationLogResponse>>
    {
        private readonly IIntegrationLogService _service;

        public IntegrationLogGetAllHandler(IIntegrationLogService service)
        {
            _service = service;
        }

        public async Task<List<IntegrationLogResponse>> Handle(IntegrationLogGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
