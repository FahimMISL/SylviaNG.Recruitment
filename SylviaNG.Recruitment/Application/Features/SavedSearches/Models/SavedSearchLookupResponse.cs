namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Models
{
    public class SavedSearchLookupResponse
    {
        public long SavedSearchId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsShared { get; set; }

        /// <summary>True when the current user owns this search - drives Edit/Delete enablement client-side.</summary>
        public bool IsOwner { get; set; }

        /// <summary>
        /// Included here (unlike ShortlistFilterLookupResponse) because applying a saved search is
        /// a client-side rehydrate of the dashboard filter form, not a server-side apply-by-id.
        /// </summary>
        public string FilterJson { get; set; } = string.Empty;
    }
}
