using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.UniversityLibraryUpdate
{
    public class UniversityLibraryUpdateCommand : IRequest<Unit>
    {
        public long UniversityLibraryItemId { get; set; }
        public UniversityLibraryItemUpdateRequest Request { get; set; }

        public UniversityLibraryUpdateCommand(long universityLibraryItemId, UniversityLibraryItemUpdateRequest request)
        {
            UniversityLibraryItemId = universityLibraryItemId;
            Request = request;
        }
    }
}
