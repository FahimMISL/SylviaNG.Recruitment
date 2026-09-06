using MediatR;

namespace SylviaNG.Recruitment.Application.Features.SpecialCategories.Commands.SpecialCategoryDelete
{
    public class SpecialCategoryDeleteCommand : IRequest<Unit>
    {
        public long SpecialCategoryId { get; set; }

        public SpecialCategoryDeleteCommand(long specialCategoryId)
        {
            SpecialCategoryId = specialCategoryId;
        }
    }
}
