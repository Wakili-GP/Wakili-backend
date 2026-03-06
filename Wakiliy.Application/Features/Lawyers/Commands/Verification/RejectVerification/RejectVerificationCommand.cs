using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Verification.RejectVerification;


public record RejectVerificationRequest(string? Note);
public class RejectVerificationCommand : IRequest<Result>
{
    public string LawyerId { get; set; } = string.Empty;
    public string? Note { get; set; }
}
