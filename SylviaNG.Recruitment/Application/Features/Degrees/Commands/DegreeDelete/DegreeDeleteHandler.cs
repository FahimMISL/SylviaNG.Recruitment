using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Degrees.Commands.DegreeDelete
{
    public class DegreeDeleteHandler : IRequestHandler<DegreeDeleteCommand, Unit>
    {
        private readonly IDegreeService _degreeService;

        public DegreeDeleteHandler(IDegreeService degreeService)
        {
            _degreeService = degreeService;
        }

        public async Task<Unit> Handle(DegreeDeleteCommand command, CancellationToken cancellationToken)
        {
            await _degreeService.DeleteAsync(command.DegreeId);
            return Unit.Value;
        }
    }
}
