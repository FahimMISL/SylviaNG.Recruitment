using MediatR;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.CandidateProfiles.Queries.UniversityLibraryGetAll
{
    public class UniversityLibraryGetAllHandler : IRequestHandler<UniversityLibraryGetAllQuery, List<UniversityLibraryItemResponse>>
    {
        private readonly IUniversityLibraryService _universityLibraryService;

        public UniversityLibraryGetAllHandler(IUniversityLibraryService universityLibraryService)
        {
            _universityLibraryService = universityLibraryService;
        }

        public async Task<List<UniversityLibraryItemResponse>> Handle(UniversityLibraryGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _universityLibraryService.GetAllAsync();
        }
    }
}
