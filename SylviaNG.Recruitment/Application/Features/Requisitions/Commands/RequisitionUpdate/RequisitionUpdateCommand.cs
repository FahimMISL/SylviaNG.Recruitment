using MediatR;
using SylviaNG.Recruitment.Application.Features.Requisitions.Models;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Commands.RequisitionUpdate
{
    public class RequisitionUpdateCommand : IRequest<Unit>
    {
        public long RequisitionId { get; set; }
        public RequisitionUpdateRequest Request { get; set; }

        public RequisitionUpdateCommand(long requisitionId, RequisitionUpdateRequest request)
        {
            RequisitionId = requisitionId;
            Request = request;
        }
    }
}
