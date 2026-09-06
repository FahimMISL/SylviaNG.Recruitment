using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletBulkDownload
{
    public class JoiningBookletBulkDownloadQuery : IRequest<JoiningBookletFileResponse>
    {
        public JoiningBookletBulkDownloadRequest Request { get; }

        public JoiningBookletBulkDownloadQuery(JoiningBookletBulkDownloadRequest request)
        {
            Request = request;
        }
    }
}
