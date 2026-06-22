using MediatR;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Commands.InsuranceDetailCreate
{
    public class InsuranceDetailCreateCommand : IRequest<long>
    {
        public InsuranceDetailCreateRequest Request { get; set; }

        public InsuranceDetailCreateCommand(InsuranceDetailCreateRequest request)
        {
            Request = request;
        }
    }
}
