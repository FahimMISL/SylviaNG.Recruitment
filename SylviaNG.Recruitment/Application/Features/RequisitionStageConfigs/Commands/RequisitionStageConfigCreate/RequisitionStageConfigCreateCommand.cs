using MediatR;
using SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Models;

namespace SylviaNG.Recruitment.Application.Features.RequisitionStageConfigs.Commands.RequisitionStageConfigCreate
{
    public class RequisitionStageConfigCreateCommand : IRequest<long>
    {
        public RequisitionStageConfigCreateRequest Request { get; set; }

        public RequisitionStageConfigCreateCommand(RequisitionStageConfigCreateRequest request)
        {
            Request = request;
        }
    }
}
