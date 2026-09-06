using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Roles.Models
{
    /// <summary>One checked cell in the permission matrix grid (Module x Action).</summary>
    public class PermissionGrantRequest
    {
        public AccessControlModuleEnum Module { get; set; }
        public PermissionActionEnum Action { get; set; }
    }
}
