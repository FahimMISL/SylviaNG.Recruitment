using MediatR;
using SylviaNG.Recruitment.Application.Features.Requisitions.Models;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Queries.RequisitionGetById
{
    public class RequisitionGetByIdQuery : IRequest<RequisitionResponse>
    {
        public long RequisitionId { get; set; }

        public RequisitionGetByIdQuery(long requisitionId)
        {
            RequisitionId = requisitionId;
        }
    }
}
