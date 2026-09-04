using MediatR;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Application.Features.SpecialCategories.Commands.SpecialCategoryCreate
{
    public class SpecialCategoryCreateHandler : IRequestHandler<SpecialCategoryCreateCommand, long>
    {
        private readonly ISpecialCategoryService _specialCategoryService;

        public SpecialCategoryCreateHandler(ISpecialCategoryService specialCategoryService)
        {
            _specialCategoryService = specialCategoryService;
        }

        public async Task<long> Handle(SpecialCategoryCreateCommand command, CancellationToken cancellationToken)
        {
            return await _specialCategoryService.CreateAsync(command.Request);
        }
    }
}
