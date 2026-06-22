using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Queries.ShortlistFilterCriteriaGetById
{
    public class ShortlistFilterCriteriaGetByIdHandler : IRequestHandler<ShortlistFilterCriteriaGetByIdQuery, ShortlistFilterCriteriaResponse>
    {
        private readonly IShortlistFilterCriteriaService _service;

        public ShortlistFilterCriteriaGetByIdHandler(IShortlistFilterCriteriaService service)
        {
            _service = service;
        }

        public async Task<ShortlistFilterCriteriaResponse> Handle(ShortlistFilterCriteriaGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ShortlistFilterCriteriaId);
        }
    }
}
