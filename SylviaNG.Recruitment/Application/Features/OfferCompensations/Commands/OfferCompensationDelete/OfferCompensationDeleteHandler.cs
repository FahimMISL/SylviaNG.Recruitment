using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Commands.OfferCompensationDelete
{
    public class OfferCompensationDeleteHandler : IRequestHandler<OfferCompensationDeleteCommand, Unit>
    {
        private readonly IOfferCompensationService _service;

        public OfferCompensationDeleteHandler(IOfferCompensationService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(OfferCompensationDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.OfferCompensationId);
            return Unit.Value;
        }
    }
}
