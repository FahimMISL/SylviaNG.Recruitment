using MediatR;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Commands.RequisitionDelete
{
    public class RequisitionDeleteCommand : IRequest<Unit>
    {
        public long RequisitionId { get; set; }

        public RequisitionDeleteCommand(long requisitionId)
        {
            RequisitionId = requisitionId;
        }
    }
}
