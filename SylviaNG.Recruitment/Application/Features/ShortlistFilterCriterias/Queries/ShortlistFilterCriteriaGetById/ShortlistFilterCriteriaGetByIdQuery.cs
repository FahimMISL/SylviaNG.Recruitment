using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Queries.ShortlistFilterCriteriaGetById
{
    public class ShortlistFilterCriteriaGetByIdQuery : IRequest<ShortlistFilterCriteriaResponse>
    {
        public long ShortlistFilterCriteriaId { get; set; }

        public ShortlistFilterCriteriaGetByIdQuery(long shortlistFilterCriteriaId)
        {
            ShortlistFilterCriteriaId = shortlistFilterCriteriaId;
        }
    }
}
