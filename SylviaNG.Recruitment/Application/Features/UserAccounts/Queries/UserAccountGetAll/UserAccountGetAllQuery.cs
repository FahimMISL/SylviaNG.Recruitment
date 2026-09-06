using MediatR;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Models;

namespace SylviaNG.Recruitment.Application.Features.UserAccounts.Queries.UserAccountGetAll
{
    public class UserAccountGetAllQuery : IRequest<List<UserAccountResponse>>
    {
    }
}
