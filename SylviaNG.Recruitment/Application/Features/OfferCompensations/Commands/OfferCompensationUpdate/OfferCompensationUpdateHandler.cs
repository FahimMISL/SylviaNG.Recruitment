using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Commands.OfferCompensationUpdate
{
    public class OfferCompensationUpdateHandler : IRequestHandler<OfferCompensationUpdateCommand, Unit>
    {
        private readonly IOfferCompensationService _service;

        public OfferCompensationUpdateHandler(IOfferCompensationService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(OfferCompensationUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.OfferCompensationId, command.Request);
            return Unit.Value;
        }
    }
}
