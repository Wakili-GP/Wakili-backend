using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Wakiliy.Application.Features.Lawyers.Commands.Update
{
    public class UpdateLawyerCommandValidator : AbstractValidator<UpdateLawyerCommand>
    {
        public UpdateLawyerCommandValidator()
        {

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.FullName)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.FullName));

            RuleFor(x => x.Address)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            RuleFor(x => x.LicenseNumber)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.LicenseNumber));

            RuleFor(x => x.Specialization)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.Specialization));

            RuleFor(x => x.YearsOfExperience)
                .GreaterThanOrEqualTo(0)
                .When(x => x.YearsOfExperience.HasValue);

            RuleFor(x => x.VerificationStatus)
                .IsInEnum()
                .When(x => x.VerificationStatus.HasValue);
        }
    }
}
