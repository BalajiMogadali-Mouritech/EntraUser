// EntraUser.Core/Features/SetPin/SetPinCommand.cs
namespace EntraUser.Core.Features.SetPin;

using EntraUser.Core.DTOs;
using MediatR;

public record SetPinCommand(
    string UserUpn,
    string Pin,
    string ConfirmPin)
    : IRequest<SetPinResultDto>;
