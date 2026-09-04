using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SpecialCategories.Commands.SpecialCategoryUpdate
{
    public class SpecialCategoryUpdateHandler : IRequestHandler<SpecialCategoryUpdateCommand, Unit>
    {
        private readonly ISpecialCategoryService _specialCategoryService;

        public SpecialCategoryUpdateHandler(ISpecialCategoryService specialCategoryService)
        {
            _specialCategoryService = specialCategoryService;
        }

        public async Task<Unit> Handle(SpecialCategoryUpdateCommand command, CancellationToken cancellationToken)
        {
            await _specialCategoryService.UpdateAsync(command.SpecialCategoryId, command.Request);
            return Unit.Value;
        }
    }
}
