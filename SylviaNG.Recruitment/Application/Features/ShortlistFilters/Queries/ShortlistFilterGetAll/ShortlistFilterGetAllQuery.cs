using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetAll
{
    public class ShortlistFilterGetAllQuery : IRequest<List<ShortlistFilterResponse>>
    {
    }
}
