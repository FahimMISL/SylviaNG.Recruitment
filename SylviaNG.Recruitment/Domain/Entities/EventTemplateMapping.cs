using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-09: resolves (RecruitmentEvent, Channel, RecipientType) to the NotificationTemplate that
/// should render for it. Unique per (Event, Channel, RecipientType) so e.g. ApplicationStatusChanged
/// + Email can have one active mapping for Candidate and a separate one for AdminHr at the same time.
/// Feature 2 (dispatch) is the consumer of this table - nothing sends yet as of Feature 1.
/// </summary>
public class EventTemplateMapping : Audit
{
    public long EventTemplateMappingId { get; set; }
    public RecruitmentEventEnum RecruitmentEvent { get; set; }
    public NotificationChannelEnum Channel { get; set; }
    public NotificationRecipientTypeEnum RecipientType { get; set; }
    public long NotificationTemplateId { get; set; }
    public bool IsActive { get; set; } = true;

    public NotificationTemplate NotificationTemplate { get; set; } = null!;
}
