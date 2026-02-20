using FluentValidation;
using Wakiliy.Domain.Enums;

namespace Wakiliy.Application.Features.Admins.Commands.UpdateAdmin
{
    public class UpdateAdminCommandValidator : AbstractValidator<UpdateAdminCommand>
    {
        public UpdateAdminCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Status)
                .Must(BeAValidStatus)
                .When(x => !string.IsNullOrEmpty(x.Status))
                .WithMessage("Invalid status. Must be 'Active', 'Inactive', 'Suspended', or 'PendingVerification'.");
        }

        private bool BeAValidStatus(string? status)
        {
            if (string.IsNullOrEmpty(status))
                return true;

            return Enum.TryParse<UserStatus>(status, true, out _);
        }
    }
}
