using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MedicalTests.Commands.MedicalTestCreate
{
    public class MedicalTestCreateHandler : IRequestHandler<MedicalTestCreateCommand, long>
    {
        private readonly IMedicalTestService _service;

        public MedicalTestCreateHandler(IMedicalTestService service)
        {
            _service = service;
        }

        public async Task<long> Handle(MedicalTestCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
