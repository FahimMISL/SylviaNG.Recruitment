using MediatR;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsUniversity.Queries.MajorSubjectUniversityGetAll
{
    public class MajorSubjectUniversityGetAllHandler : IRequestHandler<MajorSubjectUniversityGetAllQuery, List<MajorSubjectUniversityResponse>>
    {
        private readonly IMajorSubjectUniversityService _majorSubjectUniversityService;

        public MajorSubjectUniversityGetAllHandler(IMajorSubjectUniversityService majorSubjectUniversityService)
        {
            _majorSubjectUniversityService = majorSubjectUniversityService;
        }

        public async Task<List<MajorSubjectUniversityResponse>> Handle(MajorSubjectUniversityGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _majorSubjectUniversityService.GetAllAsync();
        }
    }
}
