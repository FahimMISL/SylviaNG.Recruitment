using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class InterviewEvaluationScoreRepository : Repository<InterviewEvaluationScore>, IInterviewEvaluationScoreRepository
    {
        public InterviewEvaluationScoreRepository(ApplicationDBContext context) : base(context)
        {
        }
    }
}
