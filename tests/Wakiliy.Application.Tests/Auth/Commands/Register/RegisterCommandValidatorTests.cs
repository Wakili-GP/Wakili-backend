using FluentAssertions;
using FluentValidation.TestHelper;
using Wakiliy.Application.Features.Auth.Commands.Register;

namespace Wakiliy.Application.Tests.Auth.Commands.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenFirstNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = BuildValidCommand(c => c.FirstName = string.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }


    [Fact]
    public void Validate_WhenLastNameIsEmpty_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.LastName = string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

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

    [Fact]
    public void Validate_WhenPasswordIsEmpty_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.Password = string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("ab")]
    [InlineData("12345")]
    public void Validate_WhenPasswordIsTooShort_ShouldHaveValidationError(string shortPassword)
    {
        var command = BuildValidCommand(c => c.Password = shortPassword);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_WhenAcceptTermsIsFalse_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.AcceptTerms = false);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AcceptTerms);
    }

    [Fact]
    public void Validate_WhenAcceptTermsIsNull_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.AcceptTerms = null);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AcceptTerms);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("SuperAdmin")]
    public void Validate_WhenUserTypeIsInvalid_ShouldHaveValidationError(string invalidType)
    {
        var command = BuildValidCommand(c => c.UserType = invalidType);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserType);
    }

    [Theory]
    [InlineData("Client")]
    [InlineData("Lawyer")]
    [InlineData("client")]
    [InlineData("LAWYER")]
    public void Validate_WhenUserTypeIsValid_ShouldNotHaveValidationError(string validType)
    {
        var command = BuildValidCommand(c => c.UserType = validType);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.UserType);
    }

    [Fact]
    public void Validate_WhenCommandIsFullyValid_ShouldHaveNoValidationErrors()
    {
        var command = BuildValidCommand();
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    
    private static RegisterCommand BuildValidCommand(Action<RegisterCommand>? configure = null)
    {
        var command = new RegisterCommand
        {
            FirstName = "Ahmed",
            LastName = "Ali",
            Email = "ahmed@example.com",
            Password = "Password123",
            AcceptTerms = true,
            UserType = "Client"
        };
        configure?.Invoke(command);
        return command;
    }
}
