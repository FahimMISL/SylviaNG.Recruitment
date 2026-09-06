using MediatR;
using SylviaNG.Recruitment.Application.Features.SpecialCategories.Models;

namespace SylviaNG.Recruitment.Application.Features.SpecialCategories.Commands.SpecialCategoryCreate
{
    public class SpecialCategoryCreateCommand : IRequest<long>
    {
        public SpecialCategoryCreateRequest Request { get; set; }

        public SpecialCategoryCreateCommand(SpecialCategoryCreateRequest request)
        {
            Request = request;
        }
    }
}
