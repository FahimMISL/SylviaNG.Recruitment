using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SpecialCategories.Commands.SpecialCategoryDelete
{
    public class SpecialCategoryDeleteHandler : IRequestHandler<SpecialCategoryDeleteCommand, Unit>
    {
        private readonly ISpecialCategoryService _specialCategoryService;

        public SpecialCategoryDeleteHandler(ISpecialCategoryService specialCategoryService)
        {
            _specialCategoryService = specialCategoryService;
        }

        public async Task<Unit> Handle(SpecialCategoryDeleteCommand command, CancellationToken cancellationToken)
        {
            await _specialCategoryService.DeleteAsync(command.SpecialCategoryId);
            return Unit.Value;
        }
    }
}
