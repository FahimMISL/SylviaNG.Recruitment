using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Commands.SavedSearchUpdate
{
    public class SavedSearchUpdateHandler : IRequestHandler<SavedSearchUpdateCommand, Unit>
    {
        private readonly ISavedSearchService _service;

        public SavedSearchUpdateHandler(ISavedSearchService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(SavedSearchUpdateCommand command, CancellationToken cancellationToken)
        {
            await _service.UpdateAsync(command.SavedSearchId, command.Request);
            return Unit.Value;
        }
    }
}
