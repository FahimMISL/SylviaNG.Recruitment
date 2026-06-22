using MediatR;
using SylviaNG.Recruitment.Application.Features.Nominees.Models;

namespace SylviaNG.Recruitment.Application.Features.Nominees.Queries.NomineeGetById
{
    public class NomineeGetByIdQuery : IRequest<NomineeResponse>
    {
        public long NomineeId { get; set; }

        public NomineeGetByIdQuery(long nomineeId)
        {
            NomineeId = nomineeId;
        }
    }
}
