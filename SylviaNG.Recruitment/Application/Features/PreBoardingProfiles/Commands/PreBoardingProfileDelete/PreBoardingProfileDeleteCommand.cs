using MediatR;

namespace SylviaNG.Recruitment.Application.Features.PreBoardingProfiles.Commands.PreBoardingProfileDelete
{
    public class PreBoardingProfileDeleteCommand : IRequest<Unit>
    {
        public long PreBoardingProfileId { get; set; }

        public PreBoardingProfileDeleteCommand(long preBoardingProfileId)
        {
            PreBoardingProfileId = preBoardingProfileId;
        }
    }
}
