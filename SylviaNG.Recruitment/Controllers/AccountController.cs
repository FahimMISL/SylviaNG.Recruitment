using System.Threading.RateLimiting;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SylviaNG.Recruitment.Application.Features.Users.Commands.UserCreate;
using SylviaNG.Recruitment.Application.Features.Users.Models;
using SylviaNG.Recruitment.Application.Features.Candidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.Infrastructure.Kafka;
using SylviaNG.Recruitment.Infrastructure.Services;

namespace SylviaNG.Recruitment.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("recruitment/account")]
[EnableRateLimiting("sensitive")]
public class AccountController : ControllerBase
{
    private readonly IKeycloakAdminService _keycloak;
    private readonly IMediator _mediator;
    private readonly IEmailService _emailService;
    private readonly ICandidateService _candidateService;
    private readonly IRecruitmentEventProducer _eventProducer;
    private readonly IUserNotificationService _notificationService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IKeycloakAdminService keycloak, IMediator mediator, IEmailService emailService, ICandidateService candidateService, IRecruitmentEventProducer eventProducer, IUserNotificationService notificationService, ILogger<AccountController> logger)
    {
        _keycloak = keycloak;
        _mediator = mediator;
        _emailService = emailService;
        _candidateService = candidateService;
        _eventProducer = eventProducer;
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Role))
            return Ok(new { hasError = true, decentMessage = "Full name, email, and role are required.", errorDetails = (string?)null, content = (object?)null });

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            return Ok(new { hasError = true, decentMessage = "Password must be at least 8 characters.", errorDetails = (string?)null, content = (object?)null });

        if (request.Role is not "HR" and not "Candidate")
            return Ok(new { hasError = true, decentMessage = "Role must be HR or Candidate.", errorDetails = (string?)null, content = (object?)null });

        var username = request.Email.Split('@')[0].ToLower();

        var kcResult = await _keycloak.CreateUserAsync(
            username, request.Email,
            request.FullName.Split(' ').FirstOrDefault() ?? request.FullName,
            request.FullName.Contains(' ') ? request.FullName[(request.FullName.IndexOf(' ') + 1)..] : "",
            request.Password, request.Role);

        if (!kcResult.Success)
            return Ok(new { hasError = true, decentMessage = kcResult.Error, errorDetails = (string?)null, content = (object?)null });

        var dbRequest = new UserCreateRequest
        {
            KeycloakUserId = kcResult.KeycloakUserId ?? "",
            FullName = request.FullName,
            Email = request.Email
        };

        try
        {
            await _mediator.Send(new UserCreateCommand(dbRequest));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user record in database for {Email}", request.Email);
        }

        if (request.Role == "Candidate")
        {
            try
            {
                var nameParts = request.FullName.Split(' ', 2);
                await _candidateService.CreateAsync(new CandidateCreateRequest
                {
                    KeycloakUserId = kcResult.KeycloakUserId,
                    CandidateType = CandidateTypeEnum.External,
                    FullName = request.FullName,
                    Email = request.Email,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create candidate record for {Email}", request.Email);
            }
        }

        try
        {
            await _emailService.SendAsync(
                request.Email, request.FullName,
                "Your Account Has Been Created — Smart Recruitment Platform",
                EmailTemplates.AccountCreated(request.FullName, request.Email, request.Role));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", request.Email);
        }

        try
        {
            await _eventProducer.PublishAsync(Infrastructure.Kafka.NotificationEventConsumer.TOPIC_ACCOUNT, new { action = "CREATED", keycloakUserId = kcResult.KeycloakUserId, role = request.Role, fullName = request.FullName });
            await _notificationService.NotifyUserAsync(kcResult.KeycloakUserId!, "Welcome to Smart Recruitment", $"Your {request.Role} account is ready. Start by exploring the dashboard.", Domain.Enums.UserNotificationTypeEnum.Success, "/dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish account created event for {Email}", request.Email);
        }

        return Ok(new
        {
            hasError = false,
            decentMessage = "Account created and credentials emailed.",
            errorDetails = (string?)null,
            content = new { username, email = request.Email, fullName = request.FullName, role = request.Role }
        });
    }
}

public class CreateAccountRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
