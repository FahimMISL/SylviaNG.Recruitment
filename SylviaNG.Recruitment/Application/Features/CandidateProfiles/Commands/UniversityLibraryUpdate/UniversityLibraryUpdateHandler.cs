using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.UniversityLibraryUpdate
{
    public class UniversityLibraryUpdateHandler : IRequestHandler<UniversityLibraryUpdateCommand, Unit>
    {
        private readonly IUniversityLibraryService _universityLibraryService;

        public UniversityLibraryUpdateHandler(IUniversityLibraryService universityLibraryService)
        {
            _universityLibraryService = universityLibraryService;
        }

        public async Task<Unit> Handle(UniversityLibraryUpdateCommand command, CancellationToken cancellationToken)
        {
            await _universityLibraryService.UpdateAsync(command.UniversityLibraryItemId, command.Request);
            return Unit.Value;
        }
    }
}
