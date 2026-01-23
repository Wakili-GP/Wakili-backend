using MediatR;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Commands.Delete
{
    public class DeleteLawyerCommand(string id) : IRequest<Result>
    {
        public string Id { get; } = id;
    }
}