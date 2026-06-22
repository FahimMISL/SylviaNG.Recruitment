using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Models;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Commands.AssessmentStageCreate
{
    public class AssessmentStageCreateCommand : IRequest<long>
    {
        public AssessmentStageCreateRequest Request { get; set; }

        public AssessmentStageCreateCommand(AssessmentStageCreateRequest request)
        {
            Request = request;
        }
    }
}
