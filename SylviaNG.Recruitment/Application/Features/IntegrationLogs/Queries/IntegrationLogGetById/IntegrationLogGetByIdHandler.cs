using MediatR;
using SylviaNG.Recruitment.Application.Features.IntegrationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.IntegrationLogs.Queries.IntegrationLogGetById
{
    public class IntegrationLogGetByIdHandler : IRequestHandler<IntegrationLogGetByIdQuery, IntegrationLogResponse>
    {
        private readonly IIntegrationLogService _service;

        public IntegrationLogGetByIdHandler(IIntegrationLogService service)
        {
            _service = service;
        }

        public async Task<IntegrationLogResponse> Handle(IntegrationLogGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.IntegrationLogId);
        }
    }
}
