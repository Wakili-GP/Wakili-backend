using MediatR;
using Wakiliy.Application.Features.Auth.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.Register;
public class RegisterCommand : IRequest<Result>
{
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserType UserType { get; set; } = UserType.Client;
}
