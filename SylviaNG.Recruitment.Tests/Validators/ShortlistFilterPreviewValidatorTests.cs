using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Queries.ShortlistFilterPreview;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Validators;

public class ShortlistFilterPreviewValidatorTests
{
    private readonly ShortlistFilterPreviewValidator _validator = new();

    [Fact]
    public void Validate_WithSavedFilterIdOnly_ShouldHaveNoErrors()
    {
        var query = new ShortlistFilterPreviewQuery(new ShortlistFilterPreviewRequest { ShortlistFilterId = 1, JobPostingId = 1 });

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithDefinitionOnly_ShouldHaveNoErrors()
    {
        var query = new ShortlistFilterPreviewQuery(new ShortlistFilterPreviewRequest
        {
            JobPostingId = 1,
            Definition = new ShortlistFilterDefinitionRequest
            {
                CombineWith = FilterCombinatorEnum.And,
                Criteria = new List<ShortlistFilterCriterionRequest> { new() { CriterionType = CriterionTypeEnum.MinExperienceYears, MinExperienceYears = 1 } }
            }
        });

        var result = _validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithNeitherFilterIdNorDefinition_ShouldHaveError()
    {
        var query = new ShortlistFilterPreviewQuery(new ShortlistFilterPreviewRequest { JobPostingId = 1 });

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithBothFilterIdAndDefinition_ShouldHaveError()
    {
        var query = new ShortlistFilterPreviewQuery(new ShortlistFilterPreviewRequest
        {
            ShortlistFilterId = 1,
            JobPostingId = 1,
            Definition = new ShortlistFilterDefinitionRequest()
        });

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithNoJobPostingId_ShouldHaveError()
    {
        var query = new ShortlistFilterPreviewQuery(new ShortlistFilterPreviewRequest { ShortlistFilterId = 1, JobPostingId = 0 });

        var result = _validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }
}
