// Core/Features/CheckUser/GetLocalSessionHandler.cs
namespace EntraUser.Core.Features.CheckUser;

using EntraUser.Core.Interfaces;
using EntraUser.Domain.Entities;
using MediatR;

public class GetLocalSessionHandler(IUserSessionRepository repo)
    : IRequestHandler<GetLocalSessionQuery, UserSession?>
{
    public async Task<UserSession?> Handle(
        GetLocalSessionQuery query, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(query.PreferredUpn))
            return await repo.GetByUpnAsync(query.PreferredUpn, ct);

        // Return first active session with PIN set
        var pending = await repo.GetPendingSyncAsync(ct);
        return pending.FirstOrDefault(s => s.HasPin);
    }
}