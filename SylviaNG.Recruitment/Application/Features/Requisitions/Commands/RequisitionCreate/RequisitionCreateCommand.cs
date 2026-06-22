using MediatR;
using SylviaNG.Recruitment.Application.Features.Requisitions.Models;

namespace SylviaNG.Recruitment.Application.Features.Requisitions.Commands.RequisitionCreate
{
    public class RequisitionCreateCommand : IRequest<long>
    {
        public RequisitionCreateRequest Request { get; set; }

        public RequisitionCreateCommand(RequisitionCreateRequest request)
        {
            Request = request;
        }
    }
}
