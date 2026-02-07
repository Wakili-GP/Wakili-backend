using FluentValidation;
using System;
using Wakiliy.Domain.Constants;

namespace Wakiliy.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();


        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(6);

        RuleFor(x => x.AcceptTerms)
            .Equal(true).WithMessage("You must accept the terms and conditions.");

        RuleFor(x => x.UserType)
            .NotEmpty()
            .Must(BeAValidUserType).WithMessage("Invalid user type. Must be 'Client' or 'Lawyer'.");
    }

    private bool BeAValidUserType(string userType)
    {
        return Enum.TryParse<UserType>(userType, true, out _);
    }
}
