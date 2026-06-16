using MediatR;
using Wakiliy.Application.Features.Account.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Account.Queries.GetLawyerData
{
    public class GetLawyerDataQuery : IRequest<Result<LawyerDataDto>>
    {
        public string LawyerId { get; set; }

        public GetLawyerDataQuery(string lawyerId)
        {
            LawyerId = lawyerId;
        }
    }
}
