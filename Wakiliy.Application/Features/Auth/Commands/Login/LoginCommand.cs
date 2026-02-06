using MediatR;
using Wakiliy.Application.Features.Auth.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.Login;
public class LoginCommand : IRequest<Result<LoginResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
