using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Queries.PreBoardingProfileGetById
{
    public class PreBoardingProfileGetByIdQuery : IRequest<PreBoardingProfileResponse>
    {
        public long PreBoardingProfileId { get; set; }

        public PreBoardingProfileGetByIdQuery(long preBoardingProfileId)
        {
            PreBoardingProfileId = preBoardingProfileId;
        }
    }
}
