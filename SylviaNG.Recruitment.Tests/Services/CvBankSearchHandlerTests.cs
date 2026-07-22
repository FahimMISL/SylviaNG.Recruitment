using FluentAssertions;
using FluentValidation;
using Moq;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;
using SylviaNG.Recruitment.Application.Features.CvBank.Queries.CvBankSearch;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Services;

public class CvBankSearchHandlerTests
{
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly CvBankSearchHandler _handler;

    public CvBankSearchHandlerTests()
    {
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();

        _handler = new CvBankSearchHandler(
            _candidateProfileRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object);

        _jobApplicationRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<JobApplication>());
    }

    private static CandidateProfile MakeProfile(long id, string name, string email, params string[] skills)
    {
        return new CandidateProfile
        {
            CandidateProfileId = id,
            FullName = name,
            Email = email,
            IsActive = true,
            Skills = skills.Select(s => new CandidateSkill { SkillName = s }).ToList(),
            Educations = new List<CandidateEducation>(),
            WorkExperiences = new List<CandidateWorkExperience>(),
            Certifications = new List<CandidateCertification>()
        };
    }

    [Fact]
    public async Task Handle_BooleanQuery_FiltersToMatchingCandidatesOnly()
    {
        var profiles = new List<CandidateProfile>
        {
            MakeProfile(1, "Alice", "alice@example.com", "React", "Node"),
            MakeProfile(2, "Bilal", "bilal@example.com", "Angular")
        };
        _candidateProfileRepositoryMock.Setup(r => r.GetAllActiveWithDetailsAsync()).ReturnsAsync(profiles);

        var result = await _handler.Handle(new CvBankSearchQuery(new CvBankSearchRequest { BooleanQuery = "react" }), CancellationToken.None);

        result.Data.Should().ContainSingle(r => r.CandidateProfileId == 1);
    }

    [Fact]
    public async Task Handle_NoBooleanQuery_ReturnsAllCandidatesPassingFilters()
    {
        var profiles = new List<CandidateProfile>
        {
            MakeProfile(1, "Alice", "alice@example.com"),
            MakeProfile(2, "Bilal", "bilal@example.com")
        };
        _candidateProfileRepositoryMock.Setup(r => r.GetAllActiveWithDetailsAsync()).ReturnsAsync(profiles);

        var result = await _handler.Handle(new CvBankSearchQuery(new CvBankSearchRequest()), CancellationToken.None);

        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_LocationFilter_ExcludesNonMatchingAddress()
    {
        var dhaka = MakeProfile(1, "Alice", "alice@example.com");
        dhaka.PresentAddressDetail = "Gulshan, Dhaka";
        var ctg = MakeProfile(2, "Bilal", "bilal@example.com");
        ctg.PresentAddressDetail = "Agrabad, Chattogram";

        _candidateProfileRepositoryMock.Setup(r => r.GetAllActiveWithDetailsAsync()).ReturnsAsync(new List<CandidateProfile> { dhaka, ctg });

        var result = await _handler.Handle(new CvBankSearchQuery(new CvBankSearchRequest { Location = "Dhaka" }), CancellationToken.None);

        result.Data.Should().ContainSingle(r => r.CandidateProfileId == 1);
    }

    [Fact]
    public async Task Handle_CandidateTypeFilter_UsesJobApplicationSourceHistory()
    {
        var internalCandidate = MakeProfile(1, "Alice", "alice@example.com");
        var externalCandidate = MakeProfile(2, "Bilal", "bilal@example.com");
        _candidateProfileRepositoryMock.Setup(r => r.GetAllActiveWithDetailsAsync())
            .ReturnsAsync(new List<CandidateProfile> { internalCandidate, externalCandidate });

        _jobApplicationRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<JobApplication>
        {
            new() { CandidateEmail = "alice@example.com", Source = ApplicationSourceEnum.Internal },
            new() { CandidateEmail = "bilal@example.com", Source = ApplicationSourceEnum.External }
        });

        var result = await _handler.Handle(
            new CvBankSearchQuery(new CvBankSearchRequest { CandidateType = ApplicationSourceEnum.Internal }),
            CancellationToken.None);

        result.Data.Should().ContainSingle(r => r.CandidateProfileId == 1);
    }

    [Fact]
    public async Task Handle_ResumeExtractedText_IsSearchableViaBooleanQuery()
    {
        var profile = MakeProfile(1, "Alice", "alice@example.com");
        _candidateProfileRepositoryMock.Setup(r => r.GetAllActiveWithDetailsAsync()).ReturnsAsync(new List<CandidateProfile> { profile });

        _jobApplicationRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<JobApplication>
        {
            new() { CandidateEmail = "alice@example.com", Source = ApplicationSourceEnum.External, ResumeExtractedText = "Kubernetes expert" }
        });

        var result = await _handler.Handle(new CvBankSearchQuery(new CvBankSearchRequest { BooleanQuery = "kubernetes" }), CancellationToken.None);

        result.Data.Should().ContainSingle(r => r.CandidateProfileId == 1);
    }

    [Fact]
    public async Task Handle_PhoneNumber_IsSearchableViaBooleanQuery()
    {
        var alice = MakeProfile(1, "Alice", "alice@example.com");
        alice.Phone = "01712345678";
        var bilal = MakeProfile(2, "Bilal", "bilal@example.com");
        bilal.Phone = "01898765432";
        _candidateProfileRepositoryMock.Setup(r => r.GetAllActiveWithDetailsAsync()).ReturnsAsync(new List<CandidateProfile> { alice, bilal });

        var result = await _handler.Handle(new CvBankSearchQuery(new CvBankSearchRequest { BooleanQuery = "01712345678" }), CancellationToken.None);

        result.Data.Should().ContainSingle(r => r.CandidateProfileId == 1);
    }

    [Fact]
    public async Task Handle_MalformedBooleanQuery_ThrowsValidationExceptionNotUnhandledFormatException()
    {
        _candidateProfileRepositoryMock.Setup(r => r.GetAllActiveWithDetailsAsync()).ReturnsAsync(new List<CandidateProfile>());

        var act = () => _handler.Handle(new CvBankSearchQuery(new CvBankSearchRequest { BooleanQuery = "(react AND node" }), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_RelevanceScore_RanksMoreMatchedTermsFirst()
    {
        var strongMatch = MakeProfile(1, "Alice", "alice@example.com", "React", "Node");
        var weakMatch = MakeProfile(2, "Bilal", "bilal@example.com", "React");

        _candidateProfileRepositoryMock.Setup(r => r.GetAllActiveWithDetailsAsync())
            .ReturnsAsync(new List<CandidateProfile> { weakMatch, strongMatch });

        var result = await _handler.Handle(new CvBankSearchQuery(new CvBankSearchRequest { BooleanQuery = "react OR node" }), CancellationToken.None);

        result.Data.Should().HaveCount(2);
        result.Data[0].CandidateProfileId.Should().Be(1);
        result.Data[0].RelevanceScore.Should().BeGreaterThan(result.Data[1].RelevanceScore);
    }
}
