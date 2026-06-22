using MediatR;

namespace SylviaNG.Recruitment.Application.Features.GeneratedDocuments.Commands.GeneratedDocumentDelete
{
    public class GeneratedDocumentDeleteCommand : IRequest<Unit>
    {
        public long GeneratedDocumentId { get; set; }

        public GeneratedDocumentDeleteCommand(long generatedDocumentId)
        {
            GeneratedDocumentId = generatedDocumentId;
        }
    }
}
