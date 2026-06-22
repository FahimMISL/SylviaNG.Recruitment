using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Commands.ShortlistFilterCriteriaUpdate
{
    public class ShortlistFilterCriteriaUpdateHandler : IRequestHandler<ShortlistFilterCriteriaUpdateCommand, Unit>
    {
        private readonly IShortlistFilterCriteriaService _service;

        public ShortlistFilterCriteriaUpdateHandler(IShortlistFilterCriteriaService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ShortlistFilterCriteriaUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.ShortlistFilterCriteriaId, command.Request);
            return Unit.Value;
        }
    }
}
