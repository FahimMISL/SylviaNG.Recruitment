using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateEducations.Commands.CandidateEducationCreate
{
    public class CandidateEducationCreateHandler : IRequestHandler<CandidateEducationCreateCommand, long>
    {
        private readonly ICandidateEducationService _service;

        public CandidateEducationCreateHandler(ICandidateEducationService service)
        {
            _service = service;
        }

        public async Task<long> Handle(CandidateEducationCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
