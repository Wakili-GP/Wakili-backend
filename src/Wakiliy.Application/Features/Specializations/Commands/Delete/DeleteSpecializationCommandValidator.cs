  using FluentValidation;

namespace Wakiliy.Application.Features.Specializations.Commands.Delete;

public class DeleteSpecializationCommandValidator : AbstractValidator<DeleteSpecializationCommand>
{
    public DeleteSpecializationCommandValidator()
    {
        //RuleFor(x => x.Id).GreaterThan(0);
    }
}
