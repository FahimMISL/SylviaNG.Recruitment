using MediatR;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataUpdate
{
    public class FitmentDataUpdateCommand : IRequest<Unit>
    {
        public long FitmentDataId { get; set; }
        public FitmentDataUpdateRequest Request { get; set; }

        public FitmentDataUpdateCommand(long fitmentDataId, FitmentDataUpdateRequest request)
        {
            FitmentDataId = fitmentDataId;
            Request = request;
        }
    }
}
