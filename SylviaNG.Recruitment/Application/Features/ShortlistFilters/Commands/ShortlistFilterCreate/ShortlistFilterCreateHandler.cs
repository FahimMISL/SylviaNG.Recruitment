using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterCreate
{
    public class ShortlistFilterCreateHandler : IRequestHandler<ShortlistFilterCreateCommand, long>
    {
        private readonly IShortlistFilterService _shortlistFilterService;

        public ShortlistFilterCreateHandler(IShortlistFilterService shortlistFilterService)
        {
            _shortlistFilterService = shortlistFilterService;
        }

        public async Task<long> Handle(ShortlistFilterCreateCommand command, CancellationToken cancellationToken)
        {
            return await _shortlistFilterService.CreateAsync(command.Request);
        }
    }
}
