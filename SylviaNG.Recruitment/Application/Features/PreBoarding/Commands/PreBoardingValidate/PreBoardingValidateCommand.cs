using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingValidate
{
    public class PreBoardingValidateCommand : IRequest<PreBoardingSubmissionResponse>
    {
        public long PreBoardingSubmissionId { get; set; }

        public PreBoardingValidateCommand(long preBoardingSubmissionId)
        {
            PreBoardingSubmissionId = preBoardingSubmissionId;
        }
    }
}
