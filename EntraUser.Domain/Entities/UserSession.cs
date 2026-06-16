// EntraUser.Domain/Entities/UserSession.cs
namespace EntraUser.Domain.Entities;

public class UserSession
{
    public int       Id                      { get; set; }
    public string    UserUpn                 { get; set; } = "";
    public string    DisplayName             { get; set; } = "";
    public string    ObjectId                { get; set; } = "";
    public string    NotificationEmail       { get; set; } = "";
    public bool      HasPin                  { get; set; }
    public string?   PinHash                 { get; set; }
    public bool      PasswordChangeRequired  { get; set; }
    public bool      IsActive                { get; set; } = true;
    public DateTime  CreatedAt               { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt             { get; set; }
    public DateTime? PinSetAt                { get; set; }
    public DateTime? PasswordChangedAt       { get; set; }
    public string    AccessToken             { get; set; } = "";
    public string    RefreshToken            { get; set; } = "";
    public DateTime? TokenExpiresAt          { get; set; }
}
