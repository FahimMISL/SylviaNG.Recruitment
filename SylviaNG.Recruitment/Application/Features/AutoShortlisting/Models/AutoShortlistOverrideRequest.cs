using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.AutoShortlisting.Models
{
    public class AutoShortlistOverrideRequest
    {
        /// <summary>Null clears the override, reverting to the computed pass/fail.</summary>
        public HrOverrideDecisionEnum? Decision { get; set; }
    }
}
