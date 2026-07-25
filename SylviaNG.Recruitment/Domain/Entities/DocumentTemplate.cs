using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-10: an admin-editable letter/document body for one DocumentType (Offer Letter, Appointment
/// Letter, etc). Body holds the CURRENT content - every Update also inserts a
/// DocumentTemplateVersion snapshot so history is never lost. Same shape as EP-09's
/// NotificationTemplate/NotificationTemplateVersion, kept as a separate table rather than reusing
/// NotificationTemplate because document generation (PDF, rich-text body) is a distinct concern
/// from notification dispatch (Email/SMS/InApp/Push Channel).
/// </summary>
public class DocumentTemplate : Audit
{
    public long DocumentTemplateId { get; set; }
    public DocumentTypeEnum DocumentType { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int CurrentVersionNumber { get; set; } = 1;

    public ICollection<DocumentTemplateVersion> Versions { get; set; } = new List<DocumentTemplateVersion>();
    public ICollection<OfferLetter> OfferLetters { get; set; } = new List<OfferLetter>();
    public ICollection<AppointmentLetter> AppointmentLetters { get; set; } = new List<AppointmentLetter>();
}
