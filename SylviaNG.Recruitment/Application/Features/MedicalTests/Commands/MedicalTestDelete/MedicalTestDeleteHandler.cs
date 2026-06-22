using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Commands.MedicalTestDelete
{
    public class MedicalTestDeleteHandler : IRequestHandler<MedicalTestDeleteCommand, Unit>
    {
        private readonly IMedicalTestService _service;

        public MedicalTestDeleteHandler(IMedicalTestService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(MedicalTestDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.MedicalTestId);
            return Unit.Value;
        }
    }
}
