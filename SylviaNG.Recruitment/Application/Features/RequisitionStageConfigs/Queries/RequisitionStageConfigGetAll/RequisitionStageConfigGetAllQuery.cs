using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Queries.RequisitionStageConfigGetAll
{
    public class RequisitionStageConfigGetAllQuery : IRequest<List<RequisitionStageConfigResponse>>
    {
    }
}
