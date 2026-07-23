using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        /// <summary>
        /// Matches a Core-HR-synced Employee to the logged-in candidate by email (US-005 AC1) -
        /// same soft-join-by-email precedent as JobApplication/CandidateProfile, since Employee
        /// rows arrive asynchronously via Kafka and there's no Keycloak claim carrying an
        /// employee id.
        /// </summary>
        Task<Employee?> GetByEmailAsync(string email);
    }
}
