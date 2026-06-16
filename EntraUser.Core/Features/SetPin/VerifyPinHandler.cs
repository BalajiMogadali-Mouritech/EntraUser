// Core/Features/SetPin/VerifyPinHandler.cs
namespace EntraUser.Core.Features.SetPin;

using EntraUser.Core.DTOs;
using EntraUser.Core.Interfaces;
using MediatR;

public class VerifyPinHandler(
    IUserSessionRepository repo,
    IPinService pinService)
    : IRequestHandler<VerifyPinCommand, SetPinResultDto>
{
    public async Task<SetPinResultDto> Handle(
        VerifyPinCommand cmd, CancellationToken ct)
    {
        var session = await repo.GetByUpnAsync(cmd.UserUpn, ct);

        if (session is null)
            return new SetPinResultDto(false,
                "No account found. Please sign in with TAP.");

        if (!session.HasPin ||
            string.IsNullOrEmpty(session.PinHash))
            return new SetPinResultDto(false,
                "No PIN set for this account. Please sign in with TAP.");

        var valid = pinService.VerifyPin(cmd.Pin, session.PinHash);

        if (!valid)
            return new SetPinResultDto(false,
                "Incorrect PIN. Please try again.");

        await repo.UpdateLastLoginAsync(cmd.UserUpn, ct);

        System.Diagnostics.Debug.WriteLine(
            $"[VerifyPinHandler] PIN verified · UPN={cmd.UserUpn}");

        return new SetPinResultDto(true, "PIN verified successfully.");
    }
}