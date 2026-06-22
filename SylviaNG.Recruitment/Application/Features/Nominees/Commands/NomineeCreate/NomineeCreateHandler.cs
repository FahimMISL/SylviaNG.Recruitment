using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Commands.NomineeCreate
{
    public class NomineeCreateHandler : IRequestHandler<NomineeCreateCommand, long>
    {
        private readonly INomineeService _service;

        public NomineeCreateHandler(INomineeService service)
        {
            _service = service;
        }

        public async Task<long> Handle(NomineeCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
