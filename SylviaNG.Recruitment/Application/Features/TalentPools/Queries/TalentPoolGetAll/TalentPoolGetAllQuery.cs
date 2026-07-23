using MediatR;
using SylviaNG.Recruitment.Application.Features.TalentPools.Models;

namespace SylviaNG.Recruitment.Application.Features.TalentPools.Queries.TalentPoolGetAll
{
    public class TalentPoolGetAllQuery : IRequest<List<TalentPoolResponse>>
    {
        public long? JobPostingId { get; set; }

        public TalentPoolGetAllQuery(long? jobPostingId = null)
        {
            JobPostingId = jobPostingId;
        }
    }
}
