using MediatR;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;

namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Commands.SavedSearchCreate
{
    public class SavedSearchCreateCommand : IRequest<long>
    {
        public SavedSearchCreateRequest Request { get; set; }

        public SavedSearchCreateCommand(SavedSearchCreateRequest request)
        {
            Request = request;
        }
    }
}
