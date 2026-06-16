using MediatR;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Queries.GetById;

public record GetSpecializationByIdQuery(int Id) : IRequest<Result<SpecializationResponse>>;
