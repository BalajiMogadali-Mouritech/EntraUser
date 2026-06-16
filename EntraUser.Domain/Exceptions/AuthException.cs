// EntraUser.Domain/Exceptions/AuthException.cs
namespace EntraUser.Domain.Exceptions;

public class AuthException : Exception
{
    public string? ErrorCode { get; }

    public AuthException(
        string     message,
        string?    errorCode = null,
        Exception? inner     = null)
        : base(message, inner)
    {
        ErrorCode = errorCode;
    }
}
