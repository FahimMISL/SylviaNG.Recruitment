using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterPreview
{
    public class ShortlistFilterPreviewQuery : IRequest<ShortlistFilterPreviewResponse>
    {
        public ShortlistFilterPreviewRequest Request { get; set; }

        public ShortlistFilterPreviewQuery(ShortlistFilterPreviewRequest request)
        {
            Request = request;
        }
    }
}
