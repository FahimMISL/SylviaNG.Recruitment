using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Models;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Commands.AssessmentStageUpdate
{
    public class AssessmentStageUpdateCommand : IRequest<Unit>
    {
        public long AssessmentStageId { get; set; }
        public AssessmentStageUpdateRequest Request { get; set; }

        public AssessmentStageUpdateCommand(long assessmentStageId, AssessmentStageUpdateRequest request)
        {
            AssessmentStageId = assessmentStageId;
            Request = request;
        }
    }
}
