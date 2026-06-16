using FluentValidation;

namespace Wakiliy.Application.Features.Admins.Commands.DeleteAdmin
{
    public class DeleteAdminCommandValidator : AbstractValidator<DeleteAdminCommand>
    {
        public DeleteAdminCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
