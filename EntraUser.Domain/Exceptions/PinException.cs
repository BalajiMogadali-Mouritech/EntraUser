// EntraUser.Domain/Exceptions/PinException.cs
namespace EntraUser.Domain.Exceptions;

public class PinException : Exception
{
    public string UserUpn { get; }

    public PinException(
        string     userUpn,
        string     message,
        Exception? inner = null)
        : base(message, inner)
    {
        UserUpn = userUpn;
    }
}
