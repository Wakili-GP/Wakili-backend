using MediatR;
using System.Text.Json.Serialization;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<Result>
    {
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;
    }
}
