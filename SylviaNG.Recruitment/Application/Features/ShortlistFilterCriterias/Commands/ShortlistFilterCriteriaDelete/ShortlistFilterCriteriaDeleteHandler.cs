using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Commands.ShortlistFilterCriteriaDelete
{
    public class ShortlistFilterCriteriaDeleteHandler : IRequestHandler<ShortlistFilterCriteriaDeleteCommand, Unit>
    {
        private readonly IShortlistFilterCriteriaService _service;

        public ShortlistFilterCriteriaDeleteHandler(IShortlistFilterCriteriaService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(ShortlistFilterCriteriaDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.ShortlistFilterCriteriaId);
            return Unit.Value;
        }
    }
}
