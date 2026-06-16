// EntraUser.Core/DTOs/CheckUserStatusDto.cs
namespace EntraUser.Core.DTOs;

using EntraUser.Domain.Enums;

public record CheckUserStatusDto(
    bool       TableExists,
    bool       HasPin,
    bool       PasswordChangeRequired,
    LoginState LoginState,
    string     Message);
