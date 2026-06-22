using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Queries.ShortlistFilterCriteriaGetAll
{
    public class ShortlistFilterCriteriaGetAllHandler : IRequestHandler<ShortlistFilterCriteriaGetAllQuery, List<ShortlistFilterCriteriaResponse>>
    {
        private readonly IShortlistFilterCriteriaService _service;

        public ShortlistFilterCriteriaGetAllHandler(IShortlistFilterCriteriaService service)
        {
            _service = service;
        }

        public async Task<List<ShortlistFilterCriteriaResponse>> Handle(ShortlistFilterCriteriaGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
