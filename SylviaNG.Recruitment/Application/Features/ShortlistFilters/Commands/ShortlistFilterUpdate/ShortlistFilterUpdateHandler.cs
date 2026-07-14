using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterUpdate
{
    public class ShortlistFilterUpdateHandler : IRequestHandler<ShortlistFilterUpdateCommand, Unit>
    {
        private readonly IShortlistFilterService _shortlistFilterService;

        public ShortlistFilterUpdateHandler(IShortlistFilterService shortlistFilterService)
        {
            _shortlistFilterService = shortlistFilterService;
        }

        public async Task<Unit> Handle(ShortlistFilterUpdateCommand command, CancellationToken cancellationToken)
        {
            await _shortlistFilterService.UpdateAsync(command.ShortlistFilterId, command.Request);
            return Unit.Value;
        }
    }
}
