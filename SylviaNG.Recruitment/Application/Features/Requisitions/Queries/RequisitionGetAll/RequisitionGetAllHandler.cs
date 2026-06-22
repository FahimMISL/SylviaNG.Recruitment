using MediatR;
using SylviaNG.Recruitment.Application.Features.Requisitions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Queries.RequisitionGetAll
{
    public class RequisitionGetAllHandler : IRequestHandler<RequisitionGetAllQuery, List<RequisitionResponse>>
    {
        private readonly IRequisitionService _service;

        public RequisitionGetAllHandler(IRequisitionService service)
        {
            _service = service;
        }

        public async Task<List<RequisitionResponse>> Handle(RequisitionGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
