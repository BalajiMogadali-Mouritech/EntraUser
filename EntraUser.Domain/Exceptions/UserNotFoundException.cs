// EntraUser.Domain/Exceptions/UserNotFoundException.cs
namespace EntraUser.Domain.Exceptions;

public class UserNotFoundException : Exception
{
    public string UserUpn { get; }

    public UserNotFoundException(string userUpn)
        : base($"No local session found for '{userUpn}'. " +
               "Please sign in with TAP first.")
    {
        UserUpn = userUpn;
    }
}
