using MediatR;
using SylviaNG.Recruitment.Application.Features.AccountSettings.Models;

namespace SylviaNG.Recruitment.Application.Features.AccountSettings.Queries.AccountSettingsGetMe
{
    public class AccountSettingsGetMeQuery : IRequest<AccountSettingsResponse>
    {
    }
}
