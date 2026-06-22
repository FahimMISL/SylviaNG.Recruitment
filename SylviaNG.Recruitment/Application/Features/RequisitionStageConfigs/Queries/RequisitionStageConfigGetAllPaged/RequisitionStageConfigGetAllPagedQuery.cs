using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Queries.RequisitionStageConfigGetAllPaged
{
    public class RequisitionStageConfigGetAllPagedQuery : IRequest<PagedResult<RequisitionStageConfigResponse>>
    {
        public PagedRequest Request { get; set; }

        public RequisitionStageConfigGetAllPagedQuery(PagedRequest request)
        {
            Request = request;
        }
    }
}
