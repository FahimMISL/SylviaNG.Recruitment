using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class AssessmentWorkflowRepository : Repository<AssessmentWorkflow>, IAssessmentWorkflowRepository
    {
        public AssessmentWorkflowRepository(ApplicationDBContext context) : base(context)
        {
        }
    }
}
