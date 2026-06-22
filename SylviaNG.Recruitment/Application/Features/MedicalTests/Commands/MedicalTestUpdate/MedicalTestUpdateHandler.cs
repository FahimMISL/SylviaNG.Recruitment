using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Commands.MedicalTestUpdate
{
    public class MedicalTestUpdateHandler : IRequestHandler<MedicalTestUpdateCommand, Unit>
    {
        private readonly IMedicalTestService _service;

        public MedicalTestUpdateHandler(IMedicalTestService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(MedicalTestUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.MedicalTestId, command.Request);
            return Unit.Value;
        }
    }
}
