using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface ICandidateTagRepository : IRepository<CandidateTag>
    {
        Task<List<CandidateTag>> GetAllByCandidateProfileIdAsync(long candidateProfileId);

        /// <summary>
        /// Distinct tag names already used across all candidates, for the add-tag autocomplete
        /// (US-041 AC2) and the dashboard/candidate-list filter's suggestion source. Case-insensitive
        /// contains match on <paramref name="search"/>, alphabetical, capped at <paramref name="limit"/>.
        /// </summary>
        Task<List<string>> GetDistinctTagNamesAsync(string? search, int limit);
    }
}
