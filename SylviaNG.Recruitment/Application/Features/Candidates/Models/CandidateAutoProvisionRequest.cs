namespace SylviaNG.Recruitment.Application.Features.Candidates.Models
{
    public class CandidateAutoProvisionRequest
    {
        public string KeycloakUserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
