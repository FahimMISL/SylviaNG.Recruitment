using MediatR;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Commands.InsuranceDetailDelete
{
    public class InsuranceDetailDeleteCommand : IRequest<Unit>
    {
        public long InsuranceDetailId { get; set; }

        public InsuranceDetailDeleteCommand(long insuranceDetailId)
        {
            InsuranceDetailId = insuranceDetailId;
        }
    }
}
