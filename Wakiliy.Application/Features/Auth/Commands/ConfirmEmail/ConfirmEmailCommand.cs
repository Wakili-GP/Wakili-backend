using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.ConfirmEmail;
public class ConfirmEmailCommand : IRequest<Result<string>>
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
