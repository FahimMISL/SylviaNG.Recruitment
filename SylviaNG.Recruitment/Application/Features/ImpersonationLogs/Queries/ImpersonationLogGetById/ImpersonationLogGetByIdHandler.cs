using MediatR;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Queries.ImpersonationLogGetById
{
    public class ImpersonationLogGetByIdHandler : IRequestHandler<ImpersonationLogGetByIdQuery, ImpersonationLogResponse>
    {
        private readonly IImpersonationLogService _service;

        public ImpersonationLogGetByIdHandler(IImpersonationLogService service)
        {
            _service = service;
        }

        public async Task<ImpersonationLogResponse> Handle(ImpersonationLogGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ImpersonationLogId);
        }
    }
}
