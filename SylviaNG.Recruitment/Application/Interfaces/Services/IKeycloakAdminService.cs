namespace SylviaNG.Recruitment.Application.Interfaces.Services;

public interface IKeycloakAdminService
{
    Task<KeycloakUserResult> CreateUserAsync(string username, string email, string firstName, string lastName, string password, string role);
    Task<List<string>> GetUserIdsByRoleAsync(string roleName);
    Task<bool> AssignRoleToUserAsync(string keycloakUserId, string roleName);
}

public record KeycloakUserResult(bool Success, string? KeycloakUserId, string? Error)
{
    public static KeycloakUserResult Ok(string keycloakUserId) => new(true, keycloakUserId, null);
    public static KeycloakUserResult Fail(string error) => new(false, null, error);
}
