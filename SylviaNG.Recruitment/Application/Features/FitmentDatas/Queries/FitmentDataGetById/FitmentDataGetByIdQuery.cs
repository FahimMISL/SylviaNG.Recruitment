using MediatR;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Queries.FitmentDataGetById
{
    public class FitmentDataGetByIdQuery : IRequest<FitmentDataResponse>
    {
        public long FitmentDataId { get; set; }

        public FitmentDataGetByIdQuery(long fitmentDataId)
        {
            FitmentDataId = fitmentDataId;
        }
    }
}
