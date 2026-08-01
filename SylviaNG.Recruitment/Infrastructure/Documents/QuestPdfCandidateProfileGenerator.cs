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
    /// EP-18 F3: branded candidate profile summary (US-103) for sharing with interview panels
    /// without sending the raw CV. Distinct from QuestPdfCvGenerator - adds a screening-score
    /// line, omits the "Curriculum Vitae" framing. No QR/signature block, same reasoning as the
    /// CV generator (personal profile export, not an official outbound letter).
    /// </summary>
    public class QuestPdfCandidateProfileGenerator : ICandidateProfilePdfGeneratorService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IBrandingResolverService _brandingResolverService;

        public QuestPdfCandidateProfileGenerator(IWebHostEnvironment environment, IBrandingResolverService brandingResolverService)
        {
            _environment = environment;
            _brandingResolverService = brandingResolverService;
        }

        public async Task<byte[]> Generate(CandidateProfile profile, int? screeningScore)
        {
            var branding = await _brandingResolverService.GetActiveBrandingAsync();
            var photoBytes = RelativeFileLoader.TryLoad(_environment, profile.ProfilePhotoPath);
            var logoBytes = RelativeFileLoader.TryLoad(_environment, branding.LogoFilePath);
            var referenceNumber = ReferenceNumberComponent.BuildReferenceNumber(branding, "PROFILE", DateTime.UtcNow.Year, profile.CandidateProfileId);

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

                    page.Header().Component(new DocumentHeaderComponent(branding, "Candidate Profile Summary", referenceNumber, DateTime.UtcNow, logoBytes));
                    page.Content().PaddingTop(15).Element(content => DocumentFrame.ComposeWatermarked(content, branding,
                        inner => ComposeFramedContent(inner, branding, profile, screeningScore, photoBytes)));
                    page.Footer().Element(footer => footer.Component(new DocumentFooterComponent(branding)));
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeFramedContent(IContainer container, CompanyBranding branding, CandidateProfile profile, int? screeningScore, byte[]? photoBytes)
        {
            DocumentFrame.ApplyBorder(container, branding).Padding(10).Column(column =>
            {
                column.Spacing(14);

                column.Item().Element(section => ComposeCandidateSummary(section, branding, profile, photoBytes));

                column.Item().Element(section => ComposeSection(section, branding, "Personal Details", body => body.Column(inner =>
                {
                    inner.Spacing(2);

                    if (profile.DateOfBirth.HasValue)
                        inner.Item().Text(text => { text.Span("Date of Birth: ").SemiBold(); text.Span(profile.DateOfBirth.Value.ToString("yyyy-MM-dd")); });

                    if (profile.Gender != null)
                        inner.Item().Text(text => { text.Span("Gender: ").SemiBold(); text.Span(profile.Gender.Name); });

                    var address = profile.PresentAddressDetail ?? profile.PermanentAddressDetail;
                    if (!string.IsNullOrWhiteSpace(address))
                        inner.Item().Text(text => { text.Span("Address: ").SemiBold(); text.Span(address); });

                    if (!string.IsNullOrWhiteSpace(profile.Nationality))
                        inner.Item().Text(text => { text.Span("Nationality: ").SemiBold(); text.Span(profile.Nationality); });

                    inner.Item().Text(text =>
                    {
                        text.Span("Screening Score: ").SemiBold();
                        text.Span(screeningScore.HasValue ? screeningScore.Value.ToString() : "N/A");
                    });
                })));

                var educations = profile.Educations.OrderByDescending(e => e.PassingYear).ToList();
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
                                inner.Item().Text($"{certification.CertificationName}{issuer}");
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
                            text.Span(profile.Country?.DialCode != null ? $"{profile.Country.DialCode} {profile.Phone}" : profile.Phone);
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
