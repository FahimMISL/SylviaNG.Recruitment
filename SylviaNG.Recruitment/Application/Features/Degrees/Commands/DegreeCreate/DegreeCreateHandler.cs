using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Degrees.Commands.DegreeCreate
{
    public class DegreeCreateHandler : IRequestHandler<DegreeCreateCommand, long>
    {
        private readonly IDegreeService _degreeService;

        public DegreeCreateHandler(IDegreeService degreeService)
        {
            _degreeService = degreeService;
        }

        public async Task<long> Handle(DegreeCreateCommand command, CancellationToken cancellationToken)
        {
            return await _degreeService.CreateAsync(command.Request);
        }
    }
}
