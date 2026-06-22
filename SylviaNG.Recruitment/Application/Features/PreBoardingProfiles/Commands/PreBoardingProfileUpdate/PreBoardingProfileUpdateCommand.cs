using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Commands.PreBoardingProfileUpdate
{
    public class PreBoardingProfileUpdateCommand : IRequest<Unit>
    {
        public long PreBoardingProfileId { get; set; }
        public PreBoardingProfileUpdateRequest Request { get; set; }

        public PreBoardingProfileUpdateCommand(long preBoardingProfileId, PreBoardingProfileUpdateRequest request)
        {
            PreBoardingProfileId = preBoardingProfileId;
            Request = request;
        }
    }
}
