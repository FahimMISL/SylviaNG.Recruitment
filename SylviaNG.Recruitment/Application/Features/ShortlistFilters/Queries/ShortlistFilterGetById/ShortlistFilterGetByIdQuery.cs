using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetById
{
    public class ShortlistFilterGetByIdQuery : IRequest<ShortlistFilterResponse>
    {
        public long ShortlistFilterId { get; set; }

        public ShortlistFilterGetByIdQuery(long shortlistFilterId)
        {
            ShortlistFilterId = shortlistFilterId;
        }
    }
}
