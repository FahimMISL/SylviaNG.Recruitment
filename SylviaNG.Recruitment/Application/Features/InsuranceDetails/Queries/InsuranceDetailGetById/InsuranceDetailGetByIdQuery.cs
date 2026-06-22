using MediatR;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Queries.InsuranceDetailGetById
{
    public class InsuranceDetailGetByIdQuery : IRequest<InsuranceDetailResponse>
    {
        public long InsuranceDetailId { get; set; }

        public InsuranceDetailGetByIdQuery(long insuranceDetailId)
        {
            InsuranceDetailId = insuranceDetailId;
        }
    }
}
