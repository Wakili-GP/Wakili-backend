using MediatR;
using Wakiliy.Application.Features.Admins.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Commands.UpdateAdmin
{
    public class UpdateAdminCommand : IRequest<Result<AdminDto>>
    {
        public string Id { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
