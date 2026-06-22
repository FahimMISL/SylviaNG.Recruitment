using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Models;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Queries.AssessmentStageGetById
{
    public class AssessmentStageGetByIdQuery : IRequest<AssessmentStageResponse>
    {
        public long AssessmentStageId { get; set; }

        public AssessmentStageGetByIdQuery(long assessmentStageId)
        {
            AssessmentStageId = assessmentStageId;
        }
    }
}
