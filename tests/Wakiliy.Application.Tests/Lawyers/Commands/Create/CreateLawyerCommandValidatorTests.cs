using FluentValidation.TestHelper;
using Wakiliy.Application.Features.Lawyers.Commands.Create;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Application.Tests.Lawyers.Commands.Create;

public class CreateLawyerCommandValidatorTests
{
    private readonly CreateLawyerCommandValidator _validator = new();

    // ─────────────────────────────────────────────
    //  Email
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenEmailIsEmpty_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.Email = string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WhenEmailIsInvalidFormat_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.Email = "not-an-email");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    // ─────────────────────────────────────────────
    //  PhoneNumber
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenPhoneNumberIsEmpty_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.PhoneNumber = string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_WhenPhoneNumberExceeds20Chars_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.PhoneNumber = new string('0', 21));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    // ─────────────────────────────────────────────
    //  FirstName
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenFirstNameIsEmpty_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.FirstName = string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WhenFirstNameExceeds50Chars_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.FirstName = new string('A', 51));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    // ─────────────────────────────────────────────
    //  LastName
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenLastNameIsEmpty_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.LastName = string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    // ─────────────────────────────────────────────
    //  LicenseNumber
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenLicenseNumberIsEmpty_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.LicenseNumber = string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LicenseNumber);
    }

    // ─────────────────────────────────────────────
    //  SpecializationIds
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenSpecializationIdsIsEmpty_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.SpecializationIds = new List<int>());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.SpecializationIds);
    }

    // ─────────────────────────────────────────────
    //  YearsOfExperience
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenYearsOfExperienceIsNegative_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.YearsOfExperience = -1);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.YearsOfExperience);
    }

    [Fact]
    public void Validate_WhenYearsOfExperienceIsZero_ShouldNotHaveValidationError()
    {
        var command = BuildValidCommand(c => c.YearsOfExperience = 0);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.YearsOfExperience);
    }

    // ─────────────────────────────────────────────
    //  VerificationStatus
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenVerificationStatusIsOutOfEnum_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.VerificationStatus = (VerificationStatus)999);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.VerificationStatus);
    }

    // ─────────────────────────────────────────────
    //  Valid command
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenCommandIsFullyValid_ShouldHaveNoValidationErrors()
    {
        var command = BuildValidCommand();
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ─────────────────────────────────────────────
    //  Helper
    // ─────────────────────────────────────────────

    private static CreateLaywerCommand BuildValidCommand(Action<CreateLaywerCommand>? configure = null)
    {
        var command = new CreateLaywerCommand
        {
            Email = "lawyer@example.com",
            PhoneNumber = "0501234567",
            FirstName = "Fatima",
            LastName = "Al-Rashid",
            LicenseNumber = "LIC-12345",
            SpecializationIds = new List<int> { 1, 2 },
            YearsOfExperience = 5,
            VerificationStatus = VerificationStatus.Approved
        };
        configure?.Invoke(command);
        return command;
    }
}
