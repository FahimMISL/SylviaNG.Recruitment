using MediatR;
using SylviaNG.Recruitment.Application.Features.PreBoarding.Models;

namespace SylviaNG.Recruitment.Application.Features.PreBoarding.Commands.PreBoardingSaveDraft
{
    public class PreBoardingSaveDraftCommand : IRequest<PreBoardingSubmissionResponse>
    {
        public PreBoardingSaveRequest Request { get; set; }

        public PreBoardingSaveDraftCommand(PreBoardingSaveRequest request)
        {
            Request = request;
        }
    }
}
