// Core/Features/CheckUser/GetLocalSessionQuery.cs
namespace EntraUser.Core.Features.CheckUser;

using EntraUser.Domain.Entities;
using MediatR;

public record GetLocalSessionQuery(string? PreferredUpn = null)
    : IRequest<UserSession?>;