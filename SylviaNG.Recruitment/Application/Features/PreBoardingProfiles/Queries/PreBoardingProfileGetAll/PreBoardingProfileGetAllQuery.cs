using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Queries.PreBoardingProfileGetAll
{
    public class PreBoardingProfileGetAllQuery : IRequest<List<PreBoardingProfileResponse>>
    {
    }
}
