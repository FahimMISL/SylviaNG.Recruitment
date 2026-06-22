using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Commands.AdmitCardUpdate
{
    public class AdmitCardUpdateHandler : IRequestHandler<AdmitCardUpdateCommand, Unit>
    {
        private readonly IAdmitCardService _service;

        public AdmitCardUpdateHandler(IAdmitCardService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(AdmitCardUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.AdmitCardId, command.Request);
            return Unit.Value;
        }
    }
}
