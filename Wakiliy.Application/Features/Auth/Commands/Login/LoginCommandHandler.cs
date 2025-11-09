using MediatR;
using Wakiliy.Application.Features.Auth.DTOs;

namespace Wakiliy.Application.Features.Auth.Commands.Login;
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    public Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
