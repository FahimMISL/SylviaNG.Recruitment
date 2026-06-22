using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Data;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Infrastructure.Repositories
{
    public class GeneratedDocumentRepository : Repository<GeneratedDocument>, IGeneratedDocumentRepository
    {
        public GeneratedDocumentRepository(ApplicationDBContext context) : base(context)
        {
        }
    }
}
