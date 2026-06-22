using MediatR;
using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Queries.ImpersonationLogGetAllPaged
{
    public class ImpersonationLogGetAllPagedHandler : IRequestHandler<ImpersonationLogGetAllPagedQuery, PagedResult<ImpersonationLogResponse>>
    {
        private readonly IImpersonationLogService _impersonationLogService;

        public ImpersonationLogGetAllPagedHandler(IImpersonationLogService impersonationLogService)
        {
            _impersonationLogService = impersonationLogService;
        }

        public async Task<PagedResult<ImpersonationLogResponse>> Handle(ImpersonationLogGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _impersonationLogService.GetPaginatedAsync(query.Request);
        }
    }
}
