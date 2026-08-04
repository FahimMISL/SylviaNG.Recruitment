using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// A named, persisted snapshot of an HR user's ATS dashboard filter combo (US-048). The filter
/// values themselves are never evaluated server-side - they are replayed verbatim into the
/// dashboard's existing GET query params on apply, so they are stored as an opaque JSON blob
/// rather than relational criteria rows (unlike ShortlistFilter, which IS evaluated server-side).
/// </summary>
public class SavedSearch : Audit
{
    public long SavedSearchId { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Username of the HR user who created this saved search (see ICurrentUserService).</summary>
    public string OwnerUserName { get; set; } = string.Empty;

    /// <summary>When true, visible to all HR users, not just the owner (AC4).</summary>
    public bool IsShared { get; set; }

    /// <summary>Serialized ISavedSearchFilterSnapshot from the frontend - opaque to the backend.</summary>
    public string FilterJson { get; set; } = string.Empty;
}
