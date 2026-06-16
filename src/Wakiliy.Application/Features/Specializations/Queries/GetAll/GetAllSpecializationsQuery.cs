using MediatR;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Queries.GetAll;

public record GetAllSpecializationsQuery() : IRequest<Result<List<SpecializationResponse>>>;
