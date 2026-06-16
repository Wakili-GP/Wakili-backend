using FluentValidation.TestHelper;
using Wakiliy.Application.Features.Auth.Commands.Login;

namespace Wakiliy.Application.Tests.Auth.Commands.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

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

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    [InlineData("plaintext")]
    public void Validate_WhenEmailIsInvalidFormat_ShouldHaveValidationError(string email)
    {
        var command = BuildValidCommand(c => c.Email = email);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    // ─────────────────────────────────────────────
    //  Password
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenPasswordIsEmpty_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.Password = string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
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

    private static LoginCommand BuildValidCommand(Action<LoginCommand>? configure = null)
    {
        var command = new LoginCommand
        {
            Email = "user@example.com",
            Password = "Password@123"
        };
        configure?.Invoke(command);
        return command;
    }
}
