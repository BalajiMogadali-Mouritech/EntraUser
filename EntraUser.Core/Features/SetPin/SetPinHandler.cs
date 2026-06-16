// EntraUser.Core/Features/SetPin/SetPinHandler.cs
namespace EntraUser.Core.Features.SetPin;

using EntraUser.Core.DTOs;
using EntraUser.Core.Interfaces;
using EntraUser.Domain.Exceptions;
using MediatR;

public class SetPinHandler(
    IUserSessionRepository repo,
    IPinService            pinService)
    : IRequestHandler<SetPinCommand, SetPinResultDto>
{
    public async Task<SetPinResultDto> Handle(
        SetPinCommand cmd, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cmd.Pin))
            return new SetPinResultDto(false, "PIN cannot be empty.");

        if (cmd.Pin.Length != 6)
            return new SetPinResultDto(false, "PIN must be exactly 6 digits.");

        if (!cmd.Pin.All(char.IsDigit))
            return new SetPinResultDto(false, "PIN must contain numbers only.");

        if (cmd.Pin != cmd.ConfirmPin)
            return new SetPinResultDto(false, "PINs do not match.");

        var session = await repo.GetByUpnAsync(cmd.UserUpn, ct)
            ?? throw new UserNotFoundException(cmd.UserUpn);

        var hash = pinService.HashPin(cmd.Pin);
        await repo.SetPinHashAsync(cmd.UserUpn, hash, ct);

        System.Diagnostics.Debug.WriteLine(
            $"[SetPinHandler] PIN set · UPN={cmd.UserUpn}");

        return new SetPinResultDto(true,
            "PIN set successfully. Welcome to SecuritasQuotingApp.");
    }
}
