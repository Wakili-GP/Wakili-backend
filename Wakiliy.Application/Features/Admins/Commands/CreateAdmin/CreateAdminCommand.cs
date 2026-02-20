using MediatR;
using Wakiliy.Application.Features.Admins.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Commands.CreateAdmin
{
    public class CreateAdminCommand : IRequest<Result<AdminDto>>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
