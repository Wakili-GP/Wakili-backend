using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.ResetPassword
{
    public record ResetPasswordCommand(
        string Email,
        string Code,
        string NewPassword
    ) : IRequest<Result>;

}
