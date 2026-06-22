using MediatR;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Commands.SavedSearchDelete
{
    public class SavedSearchDeleteCommand : IRequest<Unit>
    {
        public long SavedSearchId { get; set; }

        public SavedSearchDeleteCommand(long savedSearchId)
        {
            SavedSearchId = savedSearchId;
        }
    }
}
