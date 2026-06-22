using MediatR;
using SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Models;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Commands.ShortlistFilterCriteriaUpdate
{
    public class ShortlistFilterCriteriaUpdateCommand : IRequest<Unit>
    {
        public long ShortlistFilterCriteriaId { get; set; }
        public ShortlistFilterCriteriaUpdateRequest Request { get; set; }

        public ShortlistFilterCriteriaUpdateCommand(long shortlistFilterCriteriaId, ShortlistFilterCriteriaUpdateRequest request)
        {
            ShortlistFilterCriteriaId = shortlistFilterCriteriaId;
            Request = request;
        }
    }
}
