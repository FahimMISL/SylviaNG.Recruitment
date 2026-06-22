using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterGetById
{
    public class ShortlistFilterGetByIdHandler : IRequestHandler<ShortlistFilterGetByIdQuery, ShortlistFilterResponse>
    {
        private readonly IShortlistFilterService _service;

        public ShortlistFilterGetByIdHandler(IShortlistFilterService service)
        {
            _service = service;
        }

        public async Task<ShortlistFilterResponse> Handle(ShortlistFilterGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.ShortlistFilterId);
        }
    }
}
