using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Application.Features.Auth.Commands.ConfirmEmail;

namespace Wakiliy.Application.Features.Auth.Commands.ResendConfirmEmail
{
    public class ResendConfirmEmailCommandValidator : AbstractValidator<ResendConfirmEmailCommand>
    {
        public ResendConfirmEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty();
        }
    }

}