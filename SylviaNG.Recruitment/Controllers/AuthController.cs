using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SylviaNG.Recruitment.Application.Features.Candidates.Commands.CandidateAutoProvision;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;
using SylviaNG.Recruitment.Application.Features.Users.Commands.UserCreate;
using SylviaNG.Recruitment.Application.Features.Users.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;

namespace SylviaNG.Recruitment.Controllers;

[Authorize]
[ApiController]
[Route("recruitment/auth")]
public class AuthController : ControllerBase
{
    private readonly IKeycloakAdminService _keycloak;
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IKeycloakAdminService keycloak, IMediator mediator, ILogger<AuthController> logger)
    {
        _keycloak = keycloak;
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("sync-profile")]
    public async Task<IActionResult> SyncProfile()
    {
        var keycloakUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(keycloakUserId))
            return Unauthorized();

        var fullName = User.FindFirst("name")?.Value
            ?? User.FindFirst("preferred_username")?.Value ?? "";
        var email = User.FindFirst(ClaimTypes.Email)?.Value
            ?? User.FindFirst("email")?.Value ?? "";
        var username = User.FindFirst("preferred_username")?.Value ?? "";

        var assignedRole = "Candidate";
        try
        {
            await _keycloak.AssignRoleToUserAsync(keycloakUserId, assignedRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign Candidate role to {KeycloakUserId}", keycloakUserId);
            return Ok(new { hasError = true, decentMessage = "Role assignment failed.", content = (object?)null });
        }

        try
        {
            await _mediator.Send(new UserCreateCommand(new UserCreateRequest
            {
                KeycloakUserId = keycloakUserId,
                FullName = fullName,
                Email = email
            }));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "User record may already exist for {KeycloakUserId}", keycloakUserId);
        }

        try
        {
            await _mediator.Send(new CandidateAutoProvisionCommand(new CandidateAutoProvisionRequest
            {
                KeycloakUserId = keycloakUserId,
                FullName = fullName,
                Email = email
            }));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Candidate record may already exist for {KeycloakUserId}", keycloakUserId);
        }

        return Ok(new { hasError = false, decentMessage = "Profile synced.", content = new { role = assignedRole } });
    }
}
