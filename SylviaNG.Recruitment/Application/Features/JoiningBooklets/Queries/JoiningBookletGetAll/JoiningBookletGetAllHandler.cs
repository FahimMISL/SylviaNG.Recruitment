using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetAll
{
    public class JoiningBookletGetAllHandler : IRequestHandler<JoiningBookletGetAllQuery, List<JoiningBookletResponse>>
    {
        private readonly IJoiningBookletService _service;

        public JoiningBookletGetAllHandler(IJoiningBookletService service)
        {
            _service = service;
        }

        public async Task<List<JoiningBookletResponse>> Handle(JoiningBookletGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _service.GetAllAsync();
        }
    }
}
