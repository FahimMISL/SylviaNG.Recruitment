using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetById
{
    public class JoiningBookletGetByIdHandler : IRequestHandler<JoiningBookletGetByIdQuery, JoiningBookletResponse>
    {
        private readonly IJoiningBookletService _joiningBookletService;

        public JoiningBookletGetByIdHandler(IJoiningBookletService joiningBookletService)
        {
            _joiningBookletService = joiningBookletService;
        }

        public async Task<JoiningBookletResponse> Handle(JoiningBookletGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _joiningBookletService.GetByIdAsync(query.JoiningBookletId);
        }
    }
}
