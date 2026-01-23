using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.ToggleStatus
{
    public class ToggleLawyerActiveStatusCommand(string id) : IRequest<Result>
    {
        public string Id { get; } = id;
    }
}