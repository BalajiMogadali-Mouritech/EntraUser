// EntraUser.Core/DTOs/MsalAuthResultDto.cs
namespace EntraUser.Core.DTOs;

public record MsalAuthResultDto(
    bool     Success,
    string   UserUpn,
    string   DisplayName,
    string   ObjectId,
    string   AccessToken,
    string   RefreshToken,
    DateTime ExpiresAt,
    string   Message);
