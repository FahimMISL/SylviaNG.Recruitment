using MediatR;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;

namespace SylviaNG.Recruitment.Application.Features.FitmentDatas.Queries.FitmentDataGetAll
{
    public class FitmentDataGetAllQuery : IRequest<List<FitmentDataResponse>>
    {
    }
}
