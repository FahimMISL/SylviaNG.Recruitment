using MediatR;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.UniversityLibraryDelete
{
    public class UniversityLibraryDeleteCommand : IRequest<Unit>
    {
        public long UniversityLibraryItemId { get; set; }

        public UniversityLibraryDeleteCommand(long universityLibraryItemId)
        {
            UniversityLibraryItemId = universityLibraryItemId;
        }
    }
}
