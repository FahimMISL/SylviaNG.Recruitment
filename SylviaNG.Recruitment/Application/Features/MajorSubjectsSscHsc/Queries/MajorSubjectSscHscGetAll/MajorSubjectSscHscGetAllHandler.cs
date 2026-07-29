using MediatR;
using SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.MajorSubjectsSscHsc.Queries.MajorSubjectSscHscGetAll
{
    public class MajorSubjectSscHscGetAllHandler : IRequestHandler<MajorSubjectSscHscGetAllQuery, List<MajorSubjectSscHscResponse>>
    {
        private readonly IMajorSubjectSscHscService _majorSubjectSscHscService;

        public MajorSubjectSscHscGetAllHandler(IMajorSubjectSscHscService majorSubjectSscHscService)
        {
            _majorSubjectSscHscService = majorSubjectSscHscService;
        }

        public async Task<List<MajorSubjectSscHscResponse>> Handle(MajorSubjectSscHscGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _majorSubjectSscHscService.GetAllAsync();
        }
    }
}
