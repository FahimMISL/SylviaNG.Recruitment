using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Commands.SavedSearchCreate
{
    public class SavedSearchCreateHandler : IRequestHandler<SavedSearchCreateCommand, long>
    {
        private readonly ISavedSearchService _service;

        public SavedSearchCreateHandler(ISavedSearchService service)
        {
            _service = service;
        }

        public async Task<long> Handle(SavedSearchCreateCommand command, CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(command.Request);
        }
    }
}
