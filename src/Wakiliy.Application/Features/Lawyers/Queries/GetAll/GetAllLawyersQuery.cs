using MediatR;
using Wakiliy.Application.Features.Lawyers.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Lawyers.Queries.GetAll
{
    public class GetAllLawyersQuery : IRequest<Result<List<LawyerResponse>>>
    {
    }
}