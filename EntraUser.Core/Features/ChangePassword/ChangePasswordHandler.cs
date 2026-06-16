// EntraUser.Core/Features/ChangePassword/ChangePasswordHandler.cs
namespace EntraUser.Core.Features.ChangePassword;

using EntraUser.Core.DTOs;
using EntraUser.Core.Interfaces;
using MediatR;

public class ChangePasswordHandler(
    IGraphUserService      graphUser,
    IUserSessionRepository repo)
    : IRequestHandler<ChangePasswordCommand, ChangePasswordResultDto>
{
    public async Task<ChangePasswordResultDto> Handle(
        ChangePasswordCommand cmd, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cmd.NewPassword))
            return new ChangePasswordResultDto(false, "Password cannot be empty.");

        if (cmd.NewPassword.Length < 8)
            return new ChangePasswordResultDto(false,
                "Password must be at least 8 characters.");

        if (cmd.NewPassword != cmd.ConfirmPassword)
            return new ChangePasswordResultDto(false, "Passwords do not match.");

        if (!cmd.NewPassword.Any(char.IsUpper)  ||
            !cmd.NewPassword.Any(char.IsLower)  ||
            !cmd.NewPassword.Any(char.IsDigit)  ||
            !cmd.NewPassword.Any(c => !char.IsLetterOrDigit(c)))
            return new ChangePasswordResultDto(false,
                "Password must contain uppercase, lowercase, " +
                "a number and a special character.");

        try
        {
            await graphUser.UpdatePasswordAsync(
                cmd.UserObjectId, cmd.NewPassword, ct);
        }
        catch (Exception ex)
        {
            return new ChangePasswordResultDto(false,
                $"Failed to update password: {ex.Message}");
        }

        await repo.MarkPasswordChangedAsync(cmd.UserUpn, ct);

        return new ChangePasswordResultDto(true,
            "Password updated successfully. Please set your PIN.");
    }
}
