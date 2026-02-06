using System.Text.Json.Serialization;
using MediatR;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Commands.Update;

public class UpdateSpecializationCommand : IRequest<Result<SpecializationResponse>>
{
    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;

    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
