// EntraUser.Domain/Enums/LoginState.cs
namespace EntraUser.Domain.Enums;

public enum LoginState
{
    RequiresTap,
    RequiresPasswordChange,
    RequiresPin,
    Ready
}
