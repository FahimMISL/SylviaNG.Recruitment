using MediatR;
using SylviaNG.Recruitment.Application.Features.SpecialCategories.Models;

namespace SylviaNG.Recruitment.Application.Features.SpecialCategories.Queries.SpecialCategoryGetAll
{
    public class SpecialCategoryGetAllQuery : IRequest<List<SpecialCategoryResponse>>
    {
    }
}
