// Core/Features/SetPin/VerifyPinCommand.cs
namespace EntraUser.Core.Features.SetPin;

using EntraUser.Core.DTOs;
using MediatR;

public record VerifyPinCommand(
    string UserUpn,
    string Pin)
    : IRequest<SetPinResultDto>;