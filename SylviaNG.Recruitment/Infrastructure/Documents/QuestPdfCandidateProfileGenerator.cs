using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Infrastructure.Documents
{
    /// <summary>
    /// US-103: standardized, branded candidate profile summary (photo/personal details/contact,
    /// education/experience summary, skills, certifications, screening score) for sharing with
    /// interview panels without sending the raw CV. Same QuestPDF Document.Create / Compose* /
    /// TryLoadPhoto shape as QuestPdfCvGenerator and QuestPdfAdmitCardGenerator.
    /// </summary>
    public class QuestPdfCandidateProfileGenerator : ICandidateProfilePdfGeneratorService
    {
        private readonly IWebHostEnvironment _environment;

        public QuestPdfCandidateProfileGenerator(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public byte[] Generate(CandidateProfile profile, int? screeningScore)
        {
            var photoBytes = TryLoadPhoto(profile.ProfilePhotoPath);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(36);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                    page.Header().Element(header => ComposeHeader(header, profile, photoBytes));
                    page.Content().PaddingTop(15).Element(content => ComposeContent(content, profile, screeningScore));

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("SylviaNG Recruitment - Candidate Profile - Page ").FontSize(8);
                        x.CurrentPageNumber().FontSize(8);
                        x.Span(" of ").FontSize(8);
                        x.TotalPages().FontSize(8);
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeHeader(IContainer container, CandidateProfile profile, byte[]? photoBytes)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(profile.FullName).FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text("Candidate Profile Summary").FontSize(11).Italic().FontColor(Colors.Grey.Medium);

                    column.Item().PaddingTop(8).Text(text =>
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
                });

                if (photoBytes != null)
                {
                    row.ConstantItem(80).Height(90).Image(photoBytes).FitArea();
                }
            });
        }

        private static void ComposeContent(IContainer container, CandidateProfile profile, int? screeningScore)
        {
            container.Column(column =>
            {
                column.Spacing(14);

                column.Item().Element(section => ComposeSection(section, "Personal Details", body => body.Column(inner =>
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
                    column.Item().Element(section => ComposeSection(section, "Education", body => body.Table(table =>
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
                            header.Cell().Element(HeaderCell).Text("Degree");
                            header.Cell().Element(HeaderCell).Text("Institution");
                            header.Cell().Element(HeaderCell).Text("Major");
                            header.Cell().Element(HeaderCell).Text("Year");
                            header.Cell().Element(HeaderCell).Text("Result");
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
                    column.Item().Element(section => ComposeSection(section, "Work Experience", body =>
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

                    column.Item().Element(section => ComposeSection(section, "Skills", body => body.Text(skillsText)));
                }

                if (profile.Certifications.Count > 0)
                {
                    column.Item().Element(section => ComposeSection(section, "Certifications", body =>
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

        private static void ComposeSection(IContainer container, string title, Action<IContainer> body)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(1).BorderColor(Colors.Blue.Darken2).PaddingBottom(2)
                    .Text(title).FontSize(13).Bold().FontColor(Colors.Blue.Darken2);
                column.Item().PaddingTop(6).Element(body.Invoke);
            });
        }

        private static IContainer HeaderCell(IContainer container) =>
            container.DefaultTextStyle(x => x.SemiBold().FontSize(9)).PaddingBottom(3).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);

        private static IContainer BodyCell(IContainer container) =>
            container.PaddingVertical(3).DefaultTextStyle(x => x.FontSize(9));

        private byte[]? TryLoadPhoto(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            try
            {
                var physicalPath = Path.Combine(_environment.ContentRootPath, "wwwroot", relativePath.TrimStart('/'));
                return File.Exists(physicalPath) ? File.ReadAllBytes(physicalPath) : null;
            }
            catch (IOException)
            {
                return null;
            }
        }
    }
}
