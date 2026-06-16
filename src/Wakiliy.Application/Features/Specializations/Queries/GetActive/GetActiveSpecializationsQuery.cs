using MediatR;
using Wakiliy.Application.Features.Specializations.DTOs;
using Wakiliy.Domain.Responses;

namespace Wakiliy.Application.Features.Specializations.Queries.GetActive;

public record GetActiveSpecializationsQuery() : IRequest<Result<List<SpecializationOptionDto>>>;
