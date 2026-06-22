using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetById
{
    public class JoiningBookletGetByIdHandler : IRequestHandler<JoiningBookletGetByIdQuery, JoiningBookletResponse>
    {
        private readonly IJoiningBookletService _service;

        public JoiningBookletGetByIdHandler(IJoiningBookletService service)
        {
            _service = service;
        }

        public async Task<JoiningBookletResponse> Handle(JoiningBookletGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetByIdAsync(query.JoiningBookletId);
        }
    }
}
