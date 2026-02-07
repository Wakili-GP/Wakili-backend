using MediatR;
using System.Text.Json.Serialization;
using Wakiliy.Application.Features.Auth.DTOs;
using Wakiliy.Domain.Constants;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<Result>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool? AcceptTerms { get; set; }
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = "Client";
}