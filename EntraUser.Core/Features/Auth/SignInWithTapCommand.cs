// EntraUser.Core/Features/Auth/SignInWithTapCommand.cs
namespace EntraUser.Core.Features.Auth;

using EntraUser.Core.DTOs;
using MediatR;

public record SignInWithTapCommand : IRequest<MsalAuthResultDto>;
