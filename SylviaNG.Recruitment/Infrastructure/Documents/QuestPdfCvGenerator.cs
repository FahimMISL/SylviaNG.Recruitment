using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Infrastructure.Documents.Shared;

namespace SylviaNG.Recruitment.Infrastructure.Documents
{
    /// <summary>
    /// EP-18 F3: branded CV export. Renders every candidate's CV from the same section order
    /// (name/contact/photo, education, work experience, skills, certifications) so every CV
    /// pulled from the CV Bank - single or bulk - has an identical layout. No QR/signature block
    /// (a personal profile export, not an official outbound letter, unlike the branded letters).
    /// </summary>
    public class QuestPdfCvGenerator : ICvPdfGeneratorService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IBrandingResolverService _brandingResolverService;

        public QuestPdfCvGenerator(IWebHostEnvironment environment, IBrandingResolverService brandingResolverService)
        {
            _environment = environment;
            _brandingResolverService = brandingResolverService;
        }

        public async Task<byte[]> Generate(CandidateProfile profile)
        {
            var branding = await _brandingResolverService.GetActiveBrandingAsync();
            var photoBytes = RelativeFileLoader.TryLoad(_environment, profile.ProfilePhotoPath);
            var logoBytes = RelativeFileLoader.TryLoad(_environment, branding.LogoFilePath);
            var referenceNumber = ReferenceNumberComponent.BuildReferenceNumber(branding, "CV", DateTime.UtcNow.Year, profile.CandidateProfileId);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginTop(branding.MarginTop);
                    page.MarginBottom(branding.MarginBottom);
                    page.MarginLeft(branding.MarginLeft);
                    page.MarginRight(branding.MarginRight);
                    page.DefaultTextStyle(x => x
                        .FontSize(10)
                        .FontFamily(string.IsNullOrWhiteSpace(branding.FontFamily) ? "Helvetica" : branding.FontFamily)
                        .FontColor(DocumentFrame.ResolveColor(branding.SecondaryColor, Colors.Grey.Darken3)));

                    page.Header().Component(new DocumentHeaderComponent(branding, "Curriculum Vitae", referenceNumber, DateTime.UtcNow, logoBytes));
                    page.Content().PaddingTop(15).Element(content => DocumentFrame.ComposeWatermarked(content, branding,
                        inner => ComposeFramedContent(inner, branding, profile, photoBytes)));
                    page.Footer().Element(footer => footer.Component(new DocumentFooterComponent(branding)));
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeFramedContent(IContainer container, CompanyBranding branding, CandidateProfile profile, byte[]? photoBytes)
        {
            DocumentFrame.ApplyBorder(container, branding).Padding(10).Column(column =>
            {
                column.Spacing(14);

                column.Item().Element(section => ComposeCandidateSummary(section, branding, profile, photoBytes));

                var educations = profile.Educations
                    .OrderByDescending(e => e.PassingYear)
                    .ToList();

                if (educations.Count > 0)
                {
                    column.Item().Element(section => ComposeSection(section, branding, "Education", body => body.Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(cell => HeaderCell(cell, branding)).Text("Degree");
                            header.Cell().Element(cell => HeaderCell(cell, branding)).Text("Institution");
                            header.Cell().Element(cell => HeaderCell(cell, branding)).Text("Major");
                            header.Cell().Element(cell => HeaderCell(cell, branding)).Text("Year");
                            header.Cell().Element(cell => HeaderCell(cell, branding)).Text("Result");
                        });

                        foreach (var education in educations)
                        {
                            table.Cell().Element(BodyCell).Text(education.Degree.Name);
                            table.Cell().Element(BodyCell).Text(education.Institution);
                            table.Cell().Element(BodyCell).Text(education.MajorSubjectDisplay ?? "-");
                            table.Cell().Element(BodyCell).Text(education.PassingYear.ToString());
                            table.Cell().Element(BodyCell).Text(education.Result);
                        }
                    })));
                }

                var experiences = profile.WorkExperiences
                    .OrderByDescending(w => w.IsCurrent)
                    .ThenByDescending(w => w.StartDate)
                    .ToList();

                if (experiences.Count > 0)
                {
                    column.Item().Element(section => ComposeSection(section, branding, "Work Experience", body =>
                    {
                        body.Column(inner =>
                        {
                            inner.Spacing(8);

                            foreach (var experience in experiences)
                            {
                                inner.Item().Column(item =>
                                {
                                    var period = $"{experience.StartDate:MMM yyyy} - {(experience.IsCurrent ? "Present" : experience.EndDate?.ToString("MMM yyyy") ?? "-")}";

                                    item.Item().Text(text =>
                                    {
                                        text.Span($"{experience.Designation}, {experience.CompanyName}").SemiBold();
                                        text.Span($"  ({period})").FontColor(Colors.Grey.Medium).FontSize(9);
                                    });

                                    if (!string.IsNullOrWhiteSpace(experience.Location))
                                        item.Item().Text(experience.Location).FontSize(9).FontColor(Colors.Grey.Medium);

                                    item.Item().Text(experience.Responsibilities).FontSize(9.5f);
                                });
                            }
                        });
                    }));
                }

                if (profile.Skills.Count > 0)
                {
                    var skillsText = string.Join(", ", profile.Skills.Select(s =>
                        string.IsNullOrWhiteSpace(s.ProficiencyLevel) ? s.SkillName : $"{s.SkillName} ({s.ProficiencyLevel})"));

                    column.Item().Element(section => ComposeSection(section, branding, "Skills", body => body.Text(skillsText)));
                }

                if (profile.Certifications.Count > 0)
                {
                    column.Item().Element(section => ComposeSection(section, branding, "Certifications", body =>
                    {
                        body.Column(inner =>
                        {
                            inner.Spacing(4);
                            foreach (var certification in profile.Certifications)
                            {
                                var issuer = string.IsNullOrWhiteSpace(certification.IssuingOrganization) ? "" : $" - {certification.IssuingOrganization}";
                                var issueDate = certification.IssueDate.HasValue ? $" ({certification.IssueDate:MMM yyyy})" : "";
                                inner.Item().Text($"{certification.CertificationName}{issuer}{issueDate}");
                            }
                        });
                    }));
                }
            });
        }

        private static void ComposeCandidateSummary(IContainer container, CompanyBranding branding, CandidateProfile profile, byte[]? photoBytes)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(profile.FullName).FontSize(18).Bold().FontColor(DocumentFrame.ResolveColor(branding.PrimaryColor, Colors.Brown.Darken2));

                    column.Item().PaddingTop(6).Text(text =>
                    {
                        text.Span("Email: ").SemiBold();
                        text.Span(profile.Email);
                    });

                    if (!string.IsNullOrWhiteSpace(profile.Phone))
                    {
                        column.Item().Text(text =>
                        {
                            text.Span("Phone: ").SemiBold();
                            text.Span(profile.Phone);
                        });
                    }

                    var address = profile.PresentAddressDetail ?? profile.PermanentAddressDetail;
                    if (!string.IsNullOrWhiteSpace(address))
                    {
                        column.Item().Text(text =>
                        {
                            text.Span("Address: ").SemiBold();
                            text.Span(address);
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(profile.Nationality))
                    {
                        column.Item().Text(text =>
                        {
                            text.Span("Nationality: ").SemiBold();
                            text.Span(profile.Nationality);
                        });
                    }
                });

                if (photoBytes != null)
                {
                    row.ConstantItem(80).Height(90).Border(1).BorderColor(DocumentFrame.ResolveColor(branding.AccentColor, Colors.Brown.Lighten4)).Padding(2).Image(photoBytes).FitArea();
                }
            });
        }

        private static void ComposeSection(IContainer container, CompanyBranding branding, string title, Action<IContainer> body)
        {
            container.Column(column =>
            {
                var primaryColor = DocumentFrame.ResolveColor(branding.PrimaryColor, Colors.Brown.Darken2);
                column.Item().Background(primaryColor).Padding(4)
                    .Text(title).FontSize(10).Bold().FontColor(Colors.White);
                column.Item().PaddingTop(6).Element(body.Invoke);
            });
        }

        private static IContainer HeaderCell(IContainer container, CompanyBranding branding) =>
            container.Background(DocumentFrame.ResolveColor(branding.AccentColor, Colors.Brown.Lighten4))
                .DefaultTextStyle(x => x.SemiBold().FontSize(9)).Padding(3);

        private static IContainer BodyCell(IContainer container) =>
            container.PaddingVertical(3).DefaultTextStyle(x => x.FontSize(9));
    }
}
