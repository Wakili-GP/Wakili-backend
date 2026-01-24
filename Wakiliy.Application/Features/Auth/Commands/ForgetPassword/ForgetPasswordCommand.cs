using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgetPasswordCommand : IRequest<Result>
    {
        public string Email { get; set; } = string.Empty;
    }
}