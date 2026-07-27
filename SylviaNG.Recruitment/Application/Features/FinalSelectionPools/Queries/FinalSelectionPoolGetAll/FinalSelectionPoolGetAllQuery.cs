using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Queries.FinalSelectionPoolGetAll
{
    public class FinalSelectionPoolGetAllQuery : IRequest<List<FinalSelectionPoolResponse>>
    {
    }
}
