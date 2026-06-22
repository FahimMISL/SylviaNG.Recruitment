using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Commands.ShortlistFilterCriteriaCreate
{
    public class ShortlistFilterCriteriaCreateHandler : IRequestHandler<ShortlistFilterCriteriaCreateCommand, long>
    {
        private readonly IShortlistFilterCriteriaService _service;

        public ShortlistFilterCriteriaCreateHandler(IShortlistFilterCriteriaService service)
        {
            _service = service;
        }

        public async Task<long> Handle(ShortlistFilterCriteriaCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
