using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Commands.AdmitCardDelete
{
    public class AdmitCardDeleteHandler : IRequestHandler<AdmitCardDeleteCommand, Unit>
    {
        private readonly IAdmitCardService _service;

        public AdmitCardDeleteHandler(IAdmitCardService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(AdmitCardDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.AdmitCardId);
            return Unit.Value;
        }
    }
}
