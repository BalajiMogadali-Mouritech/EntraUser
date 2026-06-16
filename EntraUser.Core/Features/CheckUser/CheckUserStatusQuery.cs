// EntraUser.Core/Features/CheckUser/CheckUserStatusQuery.cs
namespace EntraUser.Core.Features.CheckUser;

using EntraUser.Core.DTOs;
using MediatR;

public record CheckUserStatusQuery : IRequest<CheckUserStatusDto>;
