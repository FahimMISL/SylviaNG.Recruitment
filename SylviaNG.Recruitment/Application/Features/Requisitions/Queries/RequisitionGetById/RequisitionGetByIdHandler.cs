using MediatR;
using SylviaNG.Recruitment.Application.Features.Requisitions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Queries.RequisitionGetById
{
    public class RequisitionGetByIdHandler : IRequestHandler<RequisitionGetByIdQuery, RequisitionResponse>
    {
        private readonly IRequisitionService _service;

        public RequisitionGetByIdHandler(IRequisitionService service)
        {
            _service = service;
        }

        public async Task<RequisitionResponse> Handle(RequisitionGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.RequisitionId);
        }
    }
}
