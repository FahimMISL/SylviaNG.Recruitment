using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingRequestCorrection
{
    public class PreBoardingRequestCorrectionCommand : IRequest<PreBoardingSubmissionResponse>
    {
        public long PreBoardingSubmissionId { get; set; }
        public PreBoardingRequestCorrectionRequest Request { get; set; }

        public PreBoardingRequestCorrectionCommand(long preBoardingSubmissionId, PreBoardingRequestCorrectionRequest request)
        {
            PreBoardingSubmissionId = preBoardingSubmissionId;
            Request = request;
        }
    }
}
