using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.AdmitCards.Commands.AdmitCardCreate
{
    public class AdmitCardCreateHandler : IRequestHandler<AdmitCardCreateCommand, long>
    {
        private readonly IAdmitCardService _service;

        public AdmitCardCreateHandler(IAdmitCardService service)
        {
            _service = service;
        }

        public async Task<long> Handle(AdmitCardCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
