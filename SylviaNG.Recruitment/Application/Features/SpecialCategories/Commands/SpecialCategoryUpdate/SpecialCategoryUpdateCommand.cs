using MediatR;
using SylviaNG.Recruitment.Application.Features.SpecialCategories.Models;

namespace SylviaNG.Recruitment.Application.Features.SpecialCategories.Commands.SpecialCategoryUpdate
{
    public class SpecialCategoryUpdateCommand : IRequest<Unit>
    {
        public long SpecialCategoryId { get; set; }
        public SpecialCategoryUpdateRequest Request { get; set; }

        public SpecialCategoryUpdateCommand(long specialCategoryId, SpecialCategoryUpdateRequest request)
        {
            SpecialCategoryId = specialCategoryId;
            Request = request;
        }
    }
}
