using MediatR;
using SylviaNG.Recruitment.Application.Features.Requisitions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Queries.RequisitionGetAllPaged
{
    public class RequisitionGetAllPagedHandler : IRequestHandler<RequisitionGetAllPagedQuery, PagedResult<RequisitionResponse>>
    {
        private readonly IRequisitionService _requisitionService;

        public RequisitionGetAllPagedHandler(IRequisitionService requisitionService)
        {
            _requisitionService = requisitionService;
        }

        public async Task<PagedResult<RequisitionResponse>> Handle(RequisitionGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _requisitionService.GetPaginatedAsync(query.Request);
        }
    }
}
