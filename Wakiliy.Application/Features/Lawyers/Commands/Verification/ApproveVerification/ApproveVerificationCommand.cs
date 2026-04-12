using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Verification.ApproveVerification;

public class ApproveVerificationCommand : IRequest<Result>
{
    public string LawyerId { get; set; } = string.Empty;
    public string AdminId { get; set; } = string.Empty;
}
