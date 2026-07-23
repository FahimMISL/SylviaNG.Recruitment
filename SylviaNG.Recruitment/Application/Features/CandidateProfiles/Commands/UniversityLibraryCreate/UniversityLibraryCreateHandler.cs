using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.UniversityLibraryCreate
{
    public class UniversityLibraryCreateHandler : IRequestHandler<UniversityLibraryCreateCommand, long>
    {
        private readonly IUniversityLibraryService _universityLibraryService;

        public UniversityLibraryCreateHandler(IUniversityLibraryService universityLibraryService)
        {
            _universityLibraryService = universityLibraryService;
        }

        public async Task<long> Handle(UniversityLibraryCreateCommand command, CancellationToken cancellationToken)
        {
            return await _universityLibraryService.CreateAsync(command.Request);
        }
    }
}
