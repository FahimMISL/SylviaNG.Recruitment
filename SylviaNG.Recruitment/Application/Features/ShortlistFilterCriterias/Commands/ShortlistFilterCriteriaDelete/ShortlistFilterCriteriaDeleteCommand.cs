using MediatR;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilterCriterias.Commands.ShortlistFilterCriteriaDelete
{
    public class ShortlistFilterCriteriaDeleteCommand : IRequest<Unit>
    {
        public long ShortlistFilterCriteriaId { get; set; }

        public ShortlistFilterCriteriaDeleteCommand(long shortlistFilterCriteriaId)
        {
            ShortlistFilterCriteriaId = shortlistFilterCriteriaId;
        }
    }
}
