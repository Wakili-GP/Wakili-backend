using FluentValidation;
using Wakiliy.Domain.Constants;

namespace Wakiliy.Application.Features.Admins.Commands.CreateAdmin
{
    public class CreateAdminCommandValidator : AbstractValidator<CreateAdminCommand>
    {
        public CreateAdminCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.Role)
                .NotEmpty()
                .Must(role => role.Equals(DefaultRoles.Admin, StringComparison.OrdinalIgnoreCase))
                .WithMessage("Role must be 'admin'.");
        }
    }
}
