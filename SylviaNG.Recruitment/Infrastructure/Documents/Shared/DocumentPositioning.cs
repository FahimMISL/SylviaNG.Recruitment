using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Documents.Shared
{
    /// <summary>EP-18 F1: maps a DocumentElementPositionEnum onto QuestPDF's alignment fluent
    /// API. Shared by QrCodeComponent and SignatureBlockComponent - both need identical
    /// TopLeft/TopRight/BottomLeft/BottomRight/Center placement.</summary>
    internal static class DocumentPositioning
    {
        public static IContainer Apply(IContainer container, DocumentElementPositionEnum position)
        {
            return position switch
            {
                DocumentElementPositionEnum.TopLeft => container.AlignLeft().AlignTop(),
                DocumentElementPositionEnum.TopRight => container.AlignRight().AlignTop(),
                DocumentElementPositionEnum.BottomLeft => container.AlignLeft().AlignBottom(),
                DocumentElementPositionEnum.BottomRight => container.AlignRight().AlignBottom(),
                DocumentElementPositionEnum.Center => container.AlignCenter().AlignMiddle(),
                _ => container
            };
        }
    }
}
