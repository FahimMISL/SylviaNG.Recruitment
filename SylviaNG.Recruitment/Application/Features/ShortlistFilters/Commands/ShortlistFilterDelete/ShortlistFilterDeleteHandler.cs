using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterDelete
{
    public class ShortlistFilterDeleteHandler : IRequestHandler<ShortlistFilterDeleteCommand, Unit>
    {
        private readonly IShortlistFilterService _service;

        public ShortlistFilterDeleteHandler(IShortlistFilterService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ShortlistFilterDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ShortlistFilterId);
            return Unit.Value;
        }
    }
}
