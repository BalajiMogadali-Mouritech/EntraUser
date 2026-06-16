// EntraUser.Core/Features/CheckUser/CheckUserStatusHandler.cs
namespace EntraUser.Core.Features.CheckUser;

using EntraUser.Core.DTOs;
using EntraUser.Core.Interfaces;
using EntraUser.Domain.Enums;
using MediatR;

public class CheckUserStatusHandler(IUserSessionRepository repo)
    : IRequestHandler<CheckUserStatusQuery, CheckUserStatusDto>
{
    public async Task<CheckUserStatusDto> Handle(
        CheckUserStatusQuery query, CancellationToken ct)
    {
        var tableExists = await repo.TableExistsAsync(ct);

        if (!tableExists)
        {
            await repo.EnsureCreatedAsync(ct);
            return new CheckUserStatusDto(
                false, false, false,
                LoginState.RequiresTap,
                "First run. Please sign in with TAP.");
        }

        return new CheckUserStatusDto(
            true, false, false,
            LoginState.RequiresTap,
            "App ready.");
    }
}
