using MediatR;
using System.Text.Json.Serialization;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Users.Commands.ToggleUserStatus
{
    public class ToggleUserStatusCommand : IRequest<Result>
    {
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;
    }
}
