using MediatR;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Commands.AssessmentStageDelete
{
    public class AssessmentStageDeleteCommand : IRequest<Unit>
    {
        public long AssessmentStageId { get; set; }

        public AssessmentStageDeleteCommand(long assessmentStageId)
        {
            AssessmentStageId = assessmentStageId;
        }
    }
}
