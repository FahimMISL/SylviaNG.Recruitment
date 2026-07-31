using MediatR;
using SylviaNG.Recruitment.Application.Features.Degrees.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.Degrees.Queries.DegreeGetAll
{
    public class DegreeGetAllHandler : IRequestHandler<DegreeGetAllQuery, List<DegreeResponse>>
    {
        private readonly IDegreeService _degreeService;

        public DegreeGetAllHandler(IDegreeService degreeService)
        {
            _degreeService = degreeService;
        }

        public async Task<List<DegreeResponse>> Handle(DegreeGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _degreeService.GetAllAsync();
        }
    }
}
