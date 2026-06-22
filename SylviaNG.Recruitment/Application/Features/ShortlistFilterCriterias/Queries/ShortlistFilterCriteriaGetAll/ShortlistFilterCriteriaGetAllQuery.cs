using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Queries.ShortlistFilterCriteriaGetAll
{
    public class ShortlistFilterCriteriaGetAllQuery : IRequest<List<ShortlistFilterCriteriaResponse>>
    {
    }
}
