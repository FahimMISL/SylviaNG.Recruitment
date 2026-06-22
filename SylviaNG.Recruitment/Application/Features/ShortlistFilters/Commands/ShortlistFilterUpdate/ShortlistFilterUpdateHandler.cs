using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterUpdate
{
    public class ShortlistFilterUpdateHandler : IRequestHandler<ShortlistFilterUpdateCommand, Unit>
    {
        private readonly IShortlistFilterService _service;

        public ShortlistFilterUpdateHandler(IShortlistFilterService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ShortlistFilterUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ShortlistFilterId, command.Request);
            return Unit.Value;
        }
    }
}
