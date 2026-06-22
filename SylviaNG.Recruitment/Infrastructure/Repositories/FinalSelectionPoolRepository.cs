using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class FinalSelectionPoolRepository : Repository<FinalSelectionPool>, IFinalSelectionPoolRepository
    {
        public FinalSelectionPoolRepository(ApplicationDBContext context) : base(context)
        {
        }
    }
}
