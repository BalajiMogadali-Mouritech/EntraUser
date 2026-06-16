// EntraUser.Core/DTOs/UserProfileDto.cs
namespace EntraUser.Core.DTOs;

public record UserProfileDto(
    string ObjectId,
    string UserPrincipalName,
    string DisplayName,
    string GivenName,
    string Surname,
    bool   ForceChangePasswordNextSignIn);
