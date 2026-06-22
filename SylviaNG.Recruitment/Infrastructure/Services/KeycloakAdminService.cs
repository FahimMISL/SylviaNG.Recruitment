using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Infrastructure.Services;

public class KeycloakAdminService : IKeycloakAdminService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private readonly string _realm;
    private readonly string _adminUser;
    private readonly string _adminPassword;

    public KeycloakAdminService(IConfiguration configuration)
    {
        _http = new HttpClient();
        var section = configuration.GetSection("Keycloak");
        var authority = section.GetValue<string>("Authority") ?? throw new ArgumentException("Keycloak:Authority missing");
        _realm = authority.Split("/realms/").Last();
        _baseUrl = authority.Replace($"/realms/{_realm}", "");
        _adminUser = section.GetValue<string>("AdminUser") ?? "admin";
        _adminPassword = section.GetValue<string>("AdminPassword") ?? "admin";
    }

    public async Task<KeycloakUserResult> CreateUserAsync(string username, string email, string firstName, string lastName, string password, string role)
    {
        var token = await GetAdminTokenAsync();
        if (token == null)
            return KeycloakUserResult.Fail("Failed to authenticate with Keycloak admin.");

        var userPayload = JsonSerializer.Serialize(new
        {
            username,
            email,
            firstName,
            lastName,
            enabled = true,
            emailVerified = true,
            credentials = new[] { new { type = "password", value = password, temporary = true } }
        });

        var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/admin/realms/{_realm}/users");
        createRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        createRequest.Content = new StringContent(userPayload, Encoding.UTF8, "application/json");

        var createResponse = await _http.SendAsync(createRequest);
        if (!createResponse.IsSuccessStatusCode)
        {
            var error = await createResponse.Content.ReadAsStringAsync();
            if (createResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
                return KeycloakUserResult.Fail("A user with this username or email already exists in Keycloak.");
            return KeycloakUserResult.Fail($"Keycloak user creation failed: {error}");
        }

        var userId = await GetUserIdAsync(token, username);
        if (userId == null)
            return KeycloakUserResult.Fail("User created but could not retrieve user ID.");

        var roleAssigned = await AssignRoleAsync(token, userId, role);
        if (!roleAssigned)
            return KeycloakUserResult.Fail($"User created but role '{role}' assignment failed.");

        return KeycloakUserResult.Ok(userId);
    }

    public async Task<List<string>> GetUserIdsByRoleAsync(string roleName)
    {
        var result = new List<string>();
        var token = await GetAdminTokenAsync();
        if (token == null) return result;

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_baseUrl}/admin/realms/{_realm}/roles/{Uri.EscapeDataString(roleName)}/users");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode) return result;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        foreach (var user in doc.RootElement.EnumerateArray())
        {
            var id = user.GetProperty("id").GetString();
            if (!string.IsNullOrEmpty(id))
                result.Add(id);
        }
        return result;
    }

    private async Task<string?> GetAdminTokenAsync()
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["username"] = _adminUser,
            ["password"] = _adminPassword,
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli"
        });

        var response = await _http.PostAsync($"{_baseUrl}/realms/master/protocol/openid-connect/token", content);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("access_token").GetString();
    }

    private async Task<string?> GetUserIdAsync(string token, string username)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/admin/realms/{_realm}/users?username={Uri.EscapeDataString(username)}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var users = doc.RootElement;
        return users.GetArrayLength() > 0 ? users[0].GetProperty("id").GetString() : null;
    }

    public async Task<bool> AssignRoleToUserAsync(string keycloakUserId, string roleName)
    {
        var token = await GetAdminTokenAsync();
        if (token == null) return false;
        return await AssignRoleAsync(token, keycloakUserId, roleName);
    }

    private async Task<bool> AssignRoleAsync(string token, string userId, string roleName)
    {
        var roleRequest = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/admin/realms/{_realm}/roles/{Uri.EscapeDataString(roleName)}");
        roleRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var roleResponse = await _http.SendAsync(roleRequest);
        if (!roleResponse.IsSuccessStatusCode) return false;

        var roleJson = await roleResponse.Content.ReadAsStringAsync();

        var assignRequest = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/admin/realms/{_realm}/users/{userId}/role-mappings/realm");
        assignRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        assignRequest.Content = new StringContent($"[{roleJson}]", Encoding.UTF8, "application/json");

        var assignResponse = await _http.SendAsync(assignRequest);
        return assignResponse.IsSuccessStatusCode;
    }
}
