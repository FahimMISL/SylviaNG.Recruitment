using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterDelete
{
    public class ShortlistFilterDeleteHandler : IRequestHandler<ShortlistFilterDeleteCommand, Unit>
    {
        private readonly IShortlistFilterService _shortlistFilterService;

        public ShortlistFilterDeleteHandler(IShortlistFilterService shortlistFilterService)
        {
            _shortlistFilterService = shortlistFilterService;
        }

        public async Task<Unit> Handle(ShortlistFilterDeleteCommand command, CancellationToken cancellationToken)
        {
            await _shortlistFilterService.DeleteAsync(command.ShortlistFilterId);
            return Unit.Value;
        }
    }
}
