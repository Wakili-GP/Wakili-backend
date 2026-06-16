using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Admins.Commands.DeleteAdmin
{
    public class DeleteAdminCommand : IRequest<Result>
    {
        public string Id { get; set; } = string.Empty;
    }
}
