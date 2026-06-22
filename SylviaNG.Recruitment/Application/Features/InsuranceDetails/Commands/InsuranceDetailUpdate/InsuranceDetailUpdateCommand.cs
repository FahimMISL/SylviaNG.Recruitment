using MediatR;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Commands.InsuranceDetailUpdate
{
    public class InsuranceDetailUpdateCommand : IRequest<Unit>
    {
        public long InsuranceDetailId { get; set; }
        public InsuranceDetailUpdateRequest Request { get; set; }

        public InsuranceDetailUpdateCommand(long insuranceDetailId, InsuranceDetailUpdateRequest request)
        {
            InsuranceDetailId = insuranceDetailId;
            Request = request;
        }
    }
}
