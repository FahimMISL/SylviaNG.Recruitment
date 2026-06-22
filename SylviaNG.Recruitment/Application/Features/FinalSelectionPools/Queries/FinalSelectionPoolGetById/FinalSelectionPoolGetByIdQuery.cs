using MediatR;
using SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Models;

namespace SylviaNG.Recruitment.Application.Features.FinalSelectionPools.Queries.FinalSelectionPoolGetById
{
    public class FinalSelectionPoolGetByIdQuery : IRequest<FinalSelectionPoolResponse>
    {
        public long FinalSelectionPoolId { get; set; }

        public FinalSelectionPoolGetByIdQuery(long finalSelectionPoolId)
        {
            FinalSelectionPoolId = finalSelectionPoolId;
        }
    }
}
