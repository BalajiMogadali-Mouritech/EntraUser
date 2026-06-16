// EntraUser.Core/Features/ChangePassword/ChangePasswordCommand.cs
namespace EntraUser.Core.Features.ChangePassword;

using EntraUser.Core.DTOs;
using MediatR;

public record ChangePasswordCommand(
    string UserObjectId,
    string UserUpn,
    string NewPassword,
    string ConfirmPassword)
    : IRequest<ChangePasswordResultDto>;
