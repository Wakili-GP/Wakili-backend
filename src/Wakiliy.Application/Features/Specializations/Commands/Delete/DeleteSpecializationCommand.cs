using MediatR;
using System.Text.Json.Serialization;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Commands.Delete;

public class DeleteSpecializationCommand : IRequest<Result>
{
    [JsonIgnore]
    public int Id { get; set; }
}
