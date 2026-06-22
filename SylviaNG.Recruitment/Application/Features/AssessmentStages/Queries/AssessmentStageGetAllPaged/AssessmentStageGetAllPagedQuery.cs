using MediatR;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.AssessmentStages.Queries.AssessmentStageGetAllPaged
{
    public class AssessmentStageGetAllPagedQuery : IRequest<PagedResult<AssessmentStageResponse>>
    {
        public PagedRequest Request { get; set; }

        public AssessmentStageGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
