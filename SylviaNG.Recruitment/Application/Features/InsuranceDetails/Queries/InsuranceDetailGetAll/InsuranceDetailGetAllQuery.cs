using MediatR;
using SylviaNG.Recruitment.Application.Features.InsuranceDetails.Models;

namespace SylviaNG.Recruitment.Application.Features.InsuranceDetails.Queries.InsuranceDetailGetAll
{
    public class InsuranceDetailGetAllQuery : IRequest<List<InsuranceDetailResponse>>
    {
    }
}
