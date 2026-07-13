using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Commands.ShortlistFilterCreate;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Validators;

public class ShortlistFilterCreateValidatorTests
{
    private readonly ShortlistFilterCreateValidator _validator = new();

    private static ShortlistFilterCreateRequest ValidRequest() => new()
    {
        Name = "Senior Engineers",
        CombineWith = FilterCombinatorEnum.And,
        Criteria = new List<ShortlistFilterCriterionRequest>
        {
            new() { CriterionType = CriterionTypeEnum.MinExperienceYears, MinExperienceYears = 5, DisplayOrder = 0 }
        }
    };

    [Fact]
    public void Validate_WithValidRequest_ShouldHaveNoErrors()
    {
        var command = new ShortlistFilterCreateCommand(ValidRequest());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Name = "";
        var command = new ShortlistFilterCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Name");
    }

    [Fact]
    public void Validate_WithNoCriteria_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Criteria = new List<ShortlistFilterCriterionRequest>();
        var command = new ShortlistFilterCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Request.Criteria");
    }

    [Fact]
    public void Validate_EducationLevelCriterionWithoutMinLevel_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Criteria = new List<ShortlistFilterCriterionRequest> { new() { CriterionType = CriterionTypeEnum.EducationLevel } };
        var command = new ShortlistFilterCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Criteria[0].MinEducationLevel");
    }

    [Fact]
    public void Validate_RequiredSkillsCriterionWithoutSkillNames_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Criteria = new List<ShortlistFilterCriterionRequest> { new() { CriterionType = CriterionTypeEnum.RequiredSkills } };
        var command = new ShortlistFilterCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Criteria[0].RequiredSkillNames");
    }

    [Fact]
    public void Validate_AgeRangeCriterionWithNeitherBoundSet_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Criteria = new List<ShortlistFilterCriterionRequest> { new() { CriterionType = CriterionTypeEnum.AgeRange } };
        var command = new ShortlistFilterCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_AgeRangeCriterionWithOnlyMinSet_ShouldHaveNoError()
    {
        var request = ValidRequest();
        request.Criteria = new List<ShortlistFilterCriterionRequest> { new() { CriterionType = CriterionTypeEnum.AgeRange, MinAge = 25 } };
        var command = new ShortlistFilterCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_DistrictCriterionWithoutDistrict_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Criteria = new List<ShortlistFilterCriterionRequest> { new() { CriterionType = CriterionTypeEnum.District } };
        var command = new ShortlistFilterCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Criteria[0].RequiredDistrict");
    }

    [Fact]
    public void Validate_MinScreeningScoreCriterionWithoutValue_ShouldHaveError()
    {
        var request = ValidRequest();
        request.Criteria = new List<ShortlistFilterCriterionRequest> { new() { CriterionType = CriterionTypeEnum.MinScreeningScore } };
        var command = new ShortlistFilterCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Criteria[0].MinScreeningScore");
    }
}
