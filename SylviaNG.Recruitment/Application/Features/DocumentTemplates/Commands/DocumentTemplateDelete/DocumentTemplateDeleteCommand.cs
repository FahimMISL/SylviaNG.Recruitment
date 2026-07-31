using MediatR;

namespace SylviaNG.Recruitment.Application.Features.DocumentTemplates.Commands.DocumentTemplateDelete
{
    public class DocumentTemplateDeleteCommand : IRequest<Unit>
    {
        public long DocumentTemplateId { get; set; }

        public DocumentTemplateDeleteCommand(long documentTemplateId)
        {
            DocumentTemplateId = documentTemplateId;
        }
    }
}
