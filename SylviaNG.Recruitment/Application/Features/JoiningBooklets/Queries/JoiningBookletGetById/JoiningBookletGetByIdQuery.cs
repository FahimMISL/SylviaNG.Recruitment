using MediatR;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;

namespace SylviaNG.Recruitment.Application.Features.JoiningBooklets.Queries.JoiningBookletGetById
{
    public class JoiningBookletGetByIdQuery : IRequest<JoiningBookletResponse>
    {
        public long JoiningBookletId { get; set; }

        public JoiningBookletGetByIdQuery(long joiningBookletId)
        {
            JoiningBookletId = joiningBookletId;
        }
    }
}
