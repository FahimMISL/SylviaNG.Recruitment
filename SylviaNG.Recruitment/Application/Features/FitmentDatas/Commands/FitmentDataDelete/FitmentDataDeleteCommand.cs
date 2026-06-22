using MediatR;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataDelete
{
    public class FitmentDataDeleteCommand : IRequest<Unit>
    {
        public long FitmentDataId { get; set; }

        public FitmentDataDeleteCommand(long fitmentDataId)
        {
            FitmentDataId = fitmentDataId;
        }
    }
}
