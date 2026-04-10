using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<Result<string>>
{
    public string UserId { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
