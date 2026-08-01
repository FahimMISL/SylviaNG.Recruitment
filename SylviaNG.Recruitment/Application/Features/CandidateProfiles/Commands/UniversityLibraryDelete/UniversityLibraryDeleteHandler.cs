using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.UniversityLibraryDelete
{
    public class UniversityLibraryDeleteHandler : IRequestHandler<UniversityLibraryDeleteCommand, Unit>
    {
        private readonly IUniversityLibraryService _universityLibraryService;

        public UniversityLibraryDeleteHandler(IUniversityLibraryService universityLibraryService)
        {
            _universityLibraryService = universityLibraryService;
        }

        public async Task<Unit> Handle(UniversityLibraryDeleteCommand command, CancellationToken cancellationToken)
        {
            await _universityLibraryService.DeleteAsync(command.UniversityLibraryItemId);
            return Unit.Value;
        }
    }
}
