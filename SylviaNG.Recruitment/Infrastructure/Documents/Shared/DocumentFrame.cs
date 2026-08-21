using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Infrastructure.Documents.Shared
{
    /// <summary>
    /// EP-18 F3: the branding-driven page-content shell (border/corner-radius per
    /// CompanyBranding.BorderStyle, plus an optional rotated watermark layer) shared by every
    /// document generator. Lifted out of QuestPdfAdmitCardGenerator/QuestPdfOfferLetterGenerator
    /// (EP-18 F2), which each hand-rolled an identical copy.
    /// </summary>
    internal static class DocumentFrame
    {
        public static IContainer ApplyBorder(IContainer container, CompanyBranding branding)
        {
            var accentColor = ResolveColor(branding.AccentColor, Colors.Grey.Lighten1);

            return branding.BorderStyle switch
            {
                DocumentBorderStyleEnum.None => container,
                DocumentBorderStyleEnum.Rounded => container.Border(1).BorderColor(accentColor).CornerRadius(branding.CornerRadius),
                DocumentBorderStyleEnum.Double => container.Border(2).BorderColor(accentColor),
                _ => container.Border(1).BorderColor(accentColor)
            };
        }

        /// <summary>Wraps composeInner in a Layers() shell: the framed/bordered content as the
        /// primary layer, plus a faint rotated CompanyName watermark overlay when
        /// BackgroundWatermarkEnabled is on.</summary>
        public static void ComposeWatermarked(IContainer container, CompanyBranding branding, Action<IContainer> composeInner)
        {
            container.Layers(layers =>
            {
                layers.PrimaryLayer().Element(primary => composeInner(primary));

                if (branding.BackgroundWatermarkEnabled && !string.IsNullOrWhiteSpace(branding.CompanyName))
                {
                    var alpha = (byte)(255 * Math.Clamp(branding.WatermarkOpacity, 0, 100) / 100);

                    // Unconstrained() is required here: a rotated large-text element's transformed
                    // bounding box is taller/wider than the page, and without this the Layers()
                    // container reports that inflated size as its own required space - silently
                    // pushing content onto a second page even though the rotated text is only ever
                    // meant to overflow/clip decoratively behind the real content.
                    layers.Layer().Unconstrained().AlignCenter().AlignMiddle().Rotate(-30)
                        .Text(branding.CompanyName).FontSize(50).Bold()
                        .FontColor(ResolveColor(branding.PrimaryColor, Colors.Grey.Lighten2).WithAlpha(alpha));
                }
            });
        }

        public static Color ResolveColor(string? hex, Color fallback) =>
            string.IsNullOrWhiteSpace(hex) ? fallback : Color.FromHex(hex);
    }
}
