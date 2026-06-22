using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterCreate
{
    public class ShortlistFilterCreateHandler : IRequestHandler<ShortlistFilterCreateCommand, long>
    {
        private readonly IShortlistFilterService _service;

        public ShortlistFilterCreateHandler(IShortlistFilterService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ShortlistFilterCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
