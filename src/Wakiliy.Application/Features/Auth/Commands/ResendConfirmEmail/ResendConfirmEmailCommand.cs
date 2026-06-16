using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.ResendConfirmEmail
{
    public class ResendConfirmEmailCommand : IRequest<Result>
    {
        public string Email { get; set; } = string.Empty;
    }
}
