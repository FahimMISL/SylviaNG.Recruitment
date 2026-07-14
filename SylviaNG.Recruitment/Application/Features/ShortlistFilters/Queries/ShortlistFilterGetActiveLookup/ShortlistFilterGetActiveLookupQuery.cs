using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetActiveLookup
{
    public class ShortlistFilterGetActiveLookupQuery : IRequest<List<ShortlistFilterLookupResponse>>
    {
    }
}
