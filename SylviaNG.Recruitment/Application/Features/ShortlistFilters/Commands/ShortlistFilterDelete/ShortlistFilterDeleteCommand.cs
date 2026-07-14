using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterDelete
{
    public class ShortlistFilterDeleteCommand : IRequest<Unit>
    {
        public long ShortlistFilterId { get; set; }

        public ShortlistFilterDeleteCommand(long shortlistFilterId)
        {
            ShortlistFilterId = shortlistFilterId;
        }
    }
}
