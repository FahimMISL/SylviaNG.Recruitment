using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Commands.SavedSearchUpdate
{
    public class SavedSearchUpdateCommand : IRequest<Unit>
    {
        public long SavedSearchId { get; set; }
        public SavedSearchUpdateRequest Request { get; set; }

        public SavedSearchUpdateCommand(long savedSearchId, SavedSearchUpdateRequest request)
        {
            SavedSearchId = savedSearchId;
            Request = request;
        }
    }
}
