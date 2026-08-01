using MediatR;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Commands.FitmentDataUpsert
{
    public class FitmentDataUpsertCommand : IRequest<FitmentDataResponse>
    {
        public FitmentDataUpsertRequest Request { get; set; }

        public FitmentDataUpsertCommand(FitmentDataUpsertRequest request)
        {
            Request = request;
        }
    }
}
