using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterUpdate
{
    public class ShortlistFilterUpdateCommand : IRequest<Unit>
    {
        public long ShortlistFilterId { get; set; }
        public ShortlistFilterUpdateRequest Request { get; set; }

        public ShortlistFilterUpdateCommand(long shortlistFilterId, ShortlistFilterUpdateRequest request)
        {
            ShortlistFilterId = shortlistFilterId;
            Request = request;
        }
    }
}
