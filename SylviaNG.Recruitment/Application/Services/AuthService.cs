using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.Auth.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// Keycloak-backed authentication (EP-15). Login proxies the ROPC grant to Keycloak's
    /// token endpoint server-side and returns the Keycloak access token; registration
    /// creates a Candidate-role realm user via the Admin REST API (US-001).
    /// </summary>
    public class AuthService : IAuthService
    {
        // Highest-privilege first: when a Keycloak user carries several known roles,
        // the response's single Role field reports the strongest one.
        private static readonly UserRoleEnum[] RolePriority = { UserRoleEnum.Admin, UserRoleEnum.HR, UserRoleEnum.Candidate };

        private const string OtpCachePrefix = "candidate-login-otp:";

        private readonly IKeycloakClient _keycloakClient;
        private readonly KeycloakSettings _keycloakSettings;
        private readonly OtpSettings _otpSettings;
        private readonly ICandidateLoginOtpRepository _candidateLoginOtpRepository;
        private readonly IPasswordResetOtpRepository _passwordResetOtpRepository;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly IMemoryCache _memoryCache;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IKeycloakClient keycloakClient,
            IOptions<KeycloakSettings> keycloakSettings,
            IOptions<OtpSettings> otpSettings,
            ICandidateLoginOtpRepository candidateLoginOtpRepository,
            IPasswordResetOtpRepository passwordResetOtpRepository,
            INotificationDispatchService notificationDispatchService,
            IMemoryCache memoryCache,
            IUnitOfWork unitOfWork,
            ILogger<AuthService> logger)
        {
            _keycloakClient = keycloakClient;
            _keycloakSettings = keycloakSettings.Value;
            _otpSettings = otpSettings.Value;
            _candidateLoginOtpRepository = candidateLoginOtpRepository;
            _passwordResetOtpRepository = passwordResetOtpRepository;
            _notificationDispatchService = notificationDispatchService;
            _memoryCache = memoryCache;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var tokenResult = await _keycloakClient.TokenAsync(request.Username, request.Password);
            var response = BuildResponseFromKeycloakToken(tokenResult);

            // EP-09 Feature 2: OTP gate applies only to a Candidate's first-ever successful login,
            // not every login - once they've completed one OTP challenge, subsequent logins skip
            // it. No-op whenever the toggle is off or the role isn't Candidate, so Admin/HR
            // behavior is unchanged.
            if (_otpSettings.Enabled && response.Role == UserRoleEnum.Candidate.ToString())
            {
                var alreadyVerifiedBefore = await _candidateLoginOtpRepository.HasEverVerifiedAsync(response.Username);
                if (alreadyVerifiedBefore)
                {
                    return response;
                }

                if (string.IsNullOrEmpty(tokenResult.RefreshToken))
                {
                    _logger.LogWarning("Candidate login for {Username} qualified for the OTP gate but Keycloak returned no refresh token - skipping the gate for this login.", response.Username);
                    return response;
                }

                return await BeginOtpChallengeAsync(response, tokenResult.RefreshToken);
            }

            return response;
        }

        private async Task<LoginResponse> BeginOtpChallengeAsync(LoginResponse candidateResponse, string refreshToken)
        {
            var challengeId = Guid.NewGuid();
            var code = GenerateOtpCode();

            var otp = new CandidateLoginOtp
            {
                ChallengeId = challengeId,
                Username = candidateResponse.Username,
                OtpCodeHash = HashOtpCode(code),
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_otpSettings.ExpiryMinutes)
            };

            await _candidateLoginOtpRepository.AddAsync(otp);
            await _unitOfWork.SaveChangesAsync();

            _memoryCache.Set(OtpCachePrefix + challengeId, refreshToken, TimeSpan.FromMinutes(_otpSettings.ExpiryMinutes));

            try
            {
                await _notificationDispatchService.DispatchAsync(
                    RecruitmentEventEnum.AccountCreatedOtp,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["CandidateName"] = candidateResponse.DisplayName,
                        ["OtpCode"] = code,
                        ["ExpiryMinutes"] = _otpSettings.ExpiryMinutes.ToString()
                    },
                    new NotificationDispatchTargets(candidateResponse.Username, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error dispatching OTP email for challenge {ChallengeId}.", challengeId);
            }

            return new LoginResponse
            {
                Username = candidateResponse.Username,
                DisplayName = candidateResponse.DisplayName,
                Role = candidateResponse.Role,
                RequiresOtp = true,
                ChallengeId = challengeId.ToString(),
                OtpExpiresAtUtc = otp.ExpiresAtUtc
            };
        }

        public async Task<LoginResponse> VerifyOtpAsync(VerifyOtpRequest request)
        {
            if (!Guid.TryParse(request.ChallengeId, out var challengeId))
                throw new OtpVerificationException();

            var otp = await _candidateLoginOtpRepository.GetByChallengeIdAsync(challengeId)
                ?? throw new OtpVerificationException();

            if (otp.Locked || otp.ConsumedAtUtc.HasValue || otp.ExpiresAtUtc < DateTime.UtcNow)
                throw new OtpVerificationException();

            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(HashOtpCode(request.Code)),
                    Encoding.UTF8.GetBytes(otp.OtpCodeHash)))
            {
                otp.AttemptCount++;
                if (otp.AttemptCount >= _otpSettings.MaxAttempts)
                    otp.Locked = true;

                _candidateLoginOtpRepository.Update(otp);
                await _unitOfWork.SaveChangesAsync();
                throw new OtpVerificationException();
            }

            var cacheKey = OtpCachePrefix + challengeId;
            if (!_memoryCache.TryGetValue(cacheKey, out string? refreshToken) || string.IsNullOrEmpty(refreshToken))
            {
                // Cache entry gone (e.g. app pool recycled mid-challenge) - the candidate must
                // start over rather than being handed a stale/nonexistent session.
                otp.Locked = true;
                _candidateLoginOtpRepository.Update(otp);
                await _unitOfWork.SaveChangesAsync();
                throw new OtpVerificationException("This login session has expired. Please log in again.");
            }

            otp.ConsumedAtUtc = DateTime.UtcNow;
            _candidateLoginOtpRepository.Update(otp);
            await _unitOfWork.SaveChangesAsync();
            _memoryCache.Remove(cacheKey);

            var tokenResult = await _keycloakClient.RefreshTokenAsync(refreshToken);
            return BuildResponseFromKeycloakToken(tokenResult);
        }

        public async Task<ResendOtpResponse> ResendOtpAsync(ResendOtpRequest request)
        {
            if (!Guid.TryParse(request.ChallengeId, out var challengeId))
                throw new OtpVerificationException();

            var otp = await _candidateLoginOtpRepository.GetByChallengeIdAsync(challengeId)
                ?? throw new OtpVerificationException();

            var cacheKey = OtpCachePrefix + challengeId;
            if (otp.Locked || otp.ConsumedAtUtc.HasValue || !_memoryCache.TryGetValue(cacheKey, out string? refreshToken) || string.IsNullOrEmpty(refreshToken))
                throw new OtpVerificationException("This login session has expired. Please log in again.");

            var code = GenerateOtpCode();
            otp.OtpCodeHash = HashOtpCode(code);
            otp.ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_otpSettings.ExpiryMinutes);
            otp.AttemptCount = 0;
            _candidateLoginOtpRepository.Update(otp);
            await _unitOfWork.SaveChangesAsync();

            _memoryCache.Set(cacheKey, refreshToken, TimeSpan.FromMinutes(_otpSettings.ExpiryMinutes));

            try
            {
                await _notificationDispatchService.DispatchAsync(
                    RecruitmentEventEnum.AccountCreatedOtp,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["CandidateName"] = otp.Username,
                        ["OtpCode"] = code,
                        ["ExpiryMinutes"] = _otpSettings.ExpiryMinutes.ToString()
                    },
                    new NotificationDispatchTargets(otp.Username, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error dispatching resend-OTP email for challenge {ChallengeId}.", challengeId);
            }

            return new ResendOtpResponse { ExpiresAtUtc = otp.ExpiresAtUtc };
        }

        private static string GenerateOtpCode()
        {
            return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        }

        private string HashOtpCode(string code)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_otpSettings.Pepper ?? string.Empty));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(code));
            return Convert.ToHexString(hash);
        }

        public async Task<LoginResponse> RefreshAsync(string refreshToken)
        {
            var tokenResult = await _keycloakClient.RefreshTokenAsync(refreshToken);
            return BuildResponseFromKeycloakToken(tokenResult);
        }

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var challengeId = Guid.NewGuid();
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(_otpSettings.ExpiryMinutes);

            try
            {
                var userId = await _keycloakClient.GetUserIdByUsernameAsync(request.Username);
                var email = await _keycloakClient.GetEmailByUserIdAsync(userId);

                if (!string.IsNullOrWhiteSpace(email))
                {
                    var code = GenerateOtpCode();
                    var otp = new PasswordResetOtp
                    {
                        ChallengeId = challengeId,
                        KeycloakUserId = userId,
                        Username = request.Username,
                        OtpCodeHash = HashOtpCode(code),
                        ExpiresAtUtc = expiresAtUtc
                    };

                    await _passwordResetOtpRepository.AddAsync(otp);
                    await _unitOfWork.SaveChangesAsync();

                    try
                    {
                        await _notificationDispatchService.DispatchAsync(
                            RecruitmentEventEnum.PasswordResetRequested,
                            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                            {
                                ["OtpCode"] = code,
                                ["ExpiryMinutes"] = _otpSettings.ExpiryMinutes.ToString()
                            },
                            new NotificationDispatchTargets(email, null));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error dispatching password-reset OTP for challenge {ChallengeId}.", challengeId);
                    }
                }
            }
            catch (Exception ex) when (ex is NotFoundException or KeycloakUnavailableException)
            {
                // Unknown username or Keycloak hiccup - fall through without persisting/sending
                // anything. The same response shape is returned either way below, so this endpoint
                // can't be used to enumerate accounts; the unresolved challenge just never verifies.
                _logger.LogInformation(ex, "Forgot-password request could not be resolved to a Keycloak user.");
            }

            return new ForgotPasswordResponse { ChallengeId = challengeId.ToString(), ExpiresAtUtc = expiresAtUtc };
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (!Guid.TryParse(request.ChallengeId, out var challengeId))
                throw new OtpVerificationException();

            var otp = await _passwordResetOtpRepository.GetByChallengeIdAsync(challengeId)
                ?? throw new OtpVerificationException();

            if (otp.Locked || otp.ConsumedAtUtc.HasValue || otp.ExpiresAtUtc < DateTime.UtcNow)
                throw new OtpVerificationException();

            if (!CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(HashOtpCode(request.OtpCode)),
                    Encoding.UTF8.GetBytes(otp.OtpCodeHash)))
            {
                otp.AttemptCount++;
                if (otp.AttemptCount >= _otpSettings.MaxAttempts)
                    otp.Locked = true;

                _passwordResetOtpRepository.Update(otp);
                await _unitOfWork.SaveChangesAsync();
                throw new OtpVerificationException();
            }

            otp.ConsumedAtUtc = DateTime.UtcNow;
            _passwordResetOtpRepository.Update(otp);
            await _unitOfWork.SaveChangesAsync();

            await _keycloakClient.ResetPasswordAsync(otp.KeycloakUserId, request.NewPassword);
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var (firstName, lastName) = SplitFullName(request.FullName);

            await _keycloakClient.CreateUserAsync(
                username: request.Email,
                email: request.Email,
                firstName: firstName,
                lastName: lastName,
                password: request.Password,
                realmRole: UserRoleEnum.Candidate.ToString(),
                requireEmailVerification: _keycloakSettings.RequireEmailVerification);

            return new RegisterResponse
            {
                Email = request.Email,
                RequiresEmailVerification = _keycloakSettings.RequireEmailVerification
            };
        }

        private LoginResponse BuildResponseFromKeycloakToken(KeycloakTokenResult tokenResult)
        {
            // The token came straight from Keycloak over the server-side channel, so it is
            // read (not signature-validated) here just to surface user metadata; real
            // validation happens on every subsequent API call via the Keycloak JWT scheme.
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenResult.AccessToken);

            var username = jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? string.Empty;
            var displayName = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? username;
            var role = ResolveKnownRole(jwt);

            return new LoginResponse
            {
                Token = tokenResult.AccessToken,
                ExpiresAtUtc = DateTime.UtcNow.AddSeconds(tokenResult.ExpiresInSeconds),
                RefreshToken = tokenResult.RefreshToken,
                Username = username,
                DisplayName = displayName,
                Role = role
            };
        }

        private static string ResolveKnownRole(JwtSecurityToken jwt)
        {
            var realmAccess = jwt.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
            if (string.IsNullOrEmpty(realmAccess))
                return string.Empty;

            try
            {
                using var json = JsonDocument.Parse(realmAccess);
                if (!json.RootElement.TryGetProperty("roles", out var rolesElement))
                    return string.Empty;

                var roles = rolesElement.EnumerateArray()
                    .Select(r => r.GetString())
                    .Where(r => !string.IsNullOrEmpty(r))
                    .ToHashSet(StringComparer.Ordinal);

                var known = RolePriority.FirstOrDefault(r => roles.Contains(r.ToString()));
                return roles.Contains(known.ToString()) ? known.ToString() : string.Empty;
            }
            catch (JsonException)
            {
                return string.Empty;
            }
        }

        private static (string FirstName, string LastName) SplitFullName(string fullName)
        {
            var trimmed = fullName.Trim();
            var spaceIdx = trimmed.IndexOf(' ');
            return spaceIdx < 0
                ? (trimmed, string.Empty)
                : (trimmed[..spaceIdx], trimmed[(spaceIdx + 1)..].Trim());
        }

    }
}
