using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletBulkDownload
{
    public class JoiningBookletBulkDownloadHandler : IRequestHandler<JoiningBookletBulkDownloadQuery, JoiningBookletFileResponse>
    {
        private readonly IJoiningBookletService _joiningBookletService;

        public JoiningBookletBulkDownloadHandler(IJoiningBookletService joiningBookletService)
        {
            _joiningBookletService = joiningBookletService;
        }

        public async Task<JoiningBookletFileResponse> Handle(JoiningBookletBulkDownloadQuery query, CancellationToken cancellationToken)
        {
            return await _joiningBookletService.BulkDownloadAsync(query.Request);
        }
    }
}
