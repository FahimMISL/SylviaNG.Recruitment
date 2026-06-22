using Microsoft.EntityFrameworkCore;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class CandidateRepository : Repository<Candidate>, ICandidateRepository
    {
        private readonly ApplicationDBContext _context;

        public CandidateRepository(ApplicationDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Candidate?> GetByEmailAsync(string email)
        {
            return await _context.Set<Candidate>()
                .FirstOrDefaultAsync(c => c.Email != null && c.Email.ToLower() == email.ToLower());
        }

        public async Task<Candidate?> GetByKeycloakUserIdAsync(string keycloakUserId)
        {
            return await _context.Set<Candidate>()
                .FirstOrDefaultAsync(c => c.KeycloakUserId == keycloakUserId);
        }
    }
}
