using MediatR;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Queries.HiringPipelineStageGetAllPaged
{
    public class HiringPipelineStageGetAllPagedQuery : IRequest<PagedResult<HiringPipelineStageResponse>>
    {
        public PagedRequest Request { get; set; }

        public HiringPipelineStageGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
