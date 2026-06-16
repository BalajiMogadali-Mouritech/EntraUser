// EntraUser.Core/Services/SessionService.cs
namespace EntraUser.Core.Services;

public class SessionService
{
    public string UserUpn     { get; private set; } = "";
    public string DisplayName { get; private set; } = "";
    public string ObjectId    { get; private set; } = "";
    public string GivenName   { get; private set; } = "";
    public string AccessToken { get; private set; } = "";
    public bool   IsSignedIn  => !string.IsNullOrWhiteSpace(UserUpn);

    public void SignIn(string upn, string displayName,
                       string objectId, string givenName,
                       string accessToken)
    {
        UserUpn     = upn;
        DisplayName = displayName;
        ObjectId    = objectId;
        GivenName   = givenName;
        AccessToken = accessToken;
    }

    public void SignOut()
    {
        UserUpn     = "";
        DisplayName = "";
        ObjectId    = "";
        GivenName   = "";
        AccessToken = "";
    }
}
