using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.UniversityLibraryGetAll
{
    public class UniversityLibraryGetAllQuery : IRequest<List<UniversityLibraryItemResponse>>
    {
    }
}
