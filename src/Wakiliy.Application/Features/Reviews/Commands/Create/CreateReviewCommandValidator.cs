using FluentValidation;

namespace Wakiliy.Application.Features.Reviews.Commands.Create;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    private static readonly double[] ValidRatings = [1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5];

    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty().WithMessage("Appointment ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        // Lawyer review validation
        RuleFor(x => x.LawyerReview)
            .NotNull().WithMessage("Lawyer review is required.");

        When(x => x.LawyerReview is not null, () =>
        {
            RuleFor(x => x.LawyerReview.Rating)
                .Must(r => ValidRatings.Contains(r))
                .WithMessage("Rating must be between 1 and 5, in 0.5 increments.");

            RuleFor(x => x.LawyerReview.Comment)
                .NotEmpty().WithMessage("Lawyer review comment is required.")
                .MaximumLength(500).WithMessage("Comment must not exceed 500 characters.");
        });


    }
}
