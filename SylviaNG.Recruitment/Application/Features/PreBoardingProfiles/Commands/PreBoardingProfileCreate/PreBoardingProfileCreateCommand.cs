using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Commands.PreBoardingProfileCreate
{
    public class PreBoardingProfileCreateCommand : IRequest<long>
    {
        public PreBoardingProfileCreateRequest Request { get; set; }

        public PreBoardingProfileCreateCommand(PreBoardingProfileCreateRequest request)
        {
            Request = request;
        }
    }
}
