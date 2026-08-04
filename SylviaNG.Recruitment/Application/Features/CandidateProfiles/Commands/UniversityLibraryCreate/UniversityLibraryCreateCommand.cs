using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Commands.UniversityLibraryCreate
{
    public class UniversityLibraryCreateCommand : IRequest<long>
    {
        public UniversityLibraryItemCreateRequest Request { get; set; }

        public UniversityLibraryCreateCommand(UniversityLibraryItemCreateRequest request)
        {
            Request = request;
        }
    }
}
