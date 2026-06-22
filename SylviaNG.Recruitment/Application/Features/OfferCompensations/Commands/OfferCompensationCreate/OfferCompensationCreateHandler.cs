using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.OfferCompensations.Commands.OfferCompensationCreate
{
    public class OfferCompensationCreateHandler : IRequestHandler<OfferCompensationCreateCommand, long>
    {
        private readonly IOfferCompensationService _service;

        public OfferCompensationCreateHandler(IOfferCompensationService service)
        {
            _service = service;
        }

        public async Task<long> Handle(OfferCompensationCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
