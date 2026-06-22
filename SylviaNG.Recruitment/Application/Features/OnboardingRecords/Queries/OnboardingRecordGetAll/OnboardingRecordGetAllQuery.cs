using MediatR;
using SylviaNG.Recruitment.Application.Features.OnboardingRecords.Models;

namespace SylviaNG.Recruitment.Application.Features.OnboardingRecords.Queries.OnboardingRecordGetAll
{
    public class OnboardingRecordGetAllQuery : IRequest<List<OnboardingRecordResponse>>
    {
    }
}
