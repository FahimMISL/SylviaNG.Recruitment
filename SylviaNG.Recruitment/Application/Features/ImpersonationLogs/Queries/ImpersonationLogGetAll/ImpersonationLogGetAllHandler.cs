using MediatR;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Queries.ImpersonationLogGetAll
{
    public class ImpersonationLogGetAllHandler : IRequestHandler<ImpersonationLogGetAllQuery, List<ImpersonationLogResponse>>
    {
        private readonly IImpersonationLogService _service;

        public ImpersonationLogGetAllHandler(IImpersonationLogService service)
        {
            _service = service;
        }

        public async Task<List<ImpersonationLogResponse>> Handle(ImpersonationLogGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
