using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Commands.SavedSearchDelete
{
    public class SavedSearchDeleteHandler : IRequestHandler<SavedSearchDeleteCommand, Unit>
    {
        private readonly ISavedSearchService _service;

        public SavedSearchDeleteHandler(ISavedSearchService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(SavedSearchDeleteCommand command, CancellationToken cancellationToken)
        {
            await _service.DeleteAsync(command.SavedSearchId);
            return Unit.Value;
        }
    }
}
