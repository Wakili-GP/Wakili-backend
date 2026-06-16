using FluentValidation.TestHelper;
using Wakiliy.Application.Features.Reviews.Commands.Create;
using Wakiliy.Application.Features.Reviews.DTOs;

namespace Wakiliy.Application.Tests.Reviews.Commands.Create;

public class CreateReviewCommandValidatorTests
{
    private readonly CreateReviewCommandValidator _validator = new();

    // ─────────────────────────────────────────────
    //  AppointmentId
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenAppointmentIdIsEmpty_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.AppointmentId = Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AppointmentId);
    }

    // ─────────────────────────────────────────────
    //  UserId
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenUserIdIsEmpty_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.UserId = string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    // ─────────────────────────────────────────────
    //  LawyerReview — null
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenLawyerReviewIsNull_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.LawyerReview = null!);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LawyerReview);
    }

    // ─────────────────────────────────────────────
    //  LawyerReview Rating — invalid values
    // ─────────────────────────────────────────────

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.3)]
    [InlineData(5.5)]
    [InlineData(6.0)]
    [InlineData(1.2)]
    public void Validate_WhenLawyerReviewRatingIsInvalid_ShouldHaveValidationError(double rating)
    {
        var command = BuildValidCommand(c => c.LawyerReview.Rating = rating);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("LawyerReview.Rating");
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    [InlineData(2.5)]
    [InlineData(3.0)]
    [InlineData(3.5)]
    [InlineData(4.0)]
    [InlineData(4.5)]
    [InlineData(5.0)]
    public void Validate_WhenLawyerReviewRatingIsValid_ShouldNotHaveValidationError(double rating)
    {
        var command = BuildValidCommand(c => c.LawyerReview.Rating = rating);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor("LawyerReview.Rating");
    }

    // ─────────────────────────────────────────────
    //  LawyerReview Comment
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenLawyerReviewCommentIsEmpty_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.LawyerReview.Comment = string.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("LawyerReview.Comment");
    }

    [Fact]
    public void Validate_WhenLawyerReviewCommentExceeds500Chars_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c => c.LawyerReview.Comment = new string('x', 501));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("LawyerReview.Comment");
    }

    [Fact]
    public void Validate_WhenLawyerReviewCommentIsExactly500Chars_ShouldNotHaveValidationError()
    {
        var command = BuildValidCommand(c => c.LawyerReview.Comment = new string('x', 500));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor("LawyerReview.Comment");
    }

    // ─────────────────────────────────────────────
    //  SystemReview — optional but validated when provided
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenSystemReviewIsNull_ShouldNotHaveAnyRelatedValidationErrors()
    {
        var command = BuildValidCommand(c => c.SystemReview = null);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.SystemReview);
    }

    [Fact]
    public void Validate_WhenSystemReviewRatingIsInvalid_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c =>
        {
            c.SystemReview = new SystemReviewDto
            {
                Rating = 0.3, // invalid
                Comment = "Fine",
                AiReview = new AiReviewDto()
            };
        });
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("SystemReview.Rating");
    }

    [Fact]
    public void Validate_WhenSystemReviewCommentExceeds500Chars_ShouldHaveValidationError()
    {
        var command = BuildValidCommand(c =>
        {
            c.SystemReview = new SystemReviewDto
            {
                Rating = 4.0,
                Comment = new string('y', 501),
                AiReview = new AiReviewDto()
            };
        });
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("SystemReview.Comment");
    }

    // ─────────────────────────────────────────────
    //  Fully valid command
    // ─────────────────────────────────────────────

    [Fact]
    public void Validate_WhenCommandIsFullyValid_ShouldHaveNoValidationErrors()
    {
        var command = BuildValidCommand();
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenCommandIsValidWithSystemReview_ShouldHaveNoValidationErrors()
    {
        var command = BuildValidCommand(c =>
        {
            c.SystemReview = new SystemReviewDto
            {
                Rating = 4.5,
                Comment = "The platform is great!",
                AiReview = new AiReviewDto { IsFlagged = false, Confidence = 0.05, Summary = "Positive" }
            };
        });
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ─────────────────────────────────────────────
    //  Helper
    // ─────────────────────────────────────────────

    private static CreateReviewCommand BuildValidCommand(Action<CreateReviewCommand>? configure = null)
    {
        var command = new CreateReviewCommand
        {
            UserId = "user-id-abc",
            AppointmentId = Guid.NewGuid(),
            LawyerReview = new LawyerReviewDto
            {
                Rating = 4.5,
                Comment = "Excellent service and very helpful.",
                AiReview = new AiReviewDto { IsFlagged = false, Confidence = 0.1, Summary = "Positive" }
            },
            SystemReview = null
        };
        configure?.Invoke(command);
        return command;
    }
}
