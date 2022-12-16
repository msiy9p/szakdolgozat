//namespace Libellus.Application.Models;

//public sealed class AuthorisationResult
//{
//    public static readonly AuthorisationResult Succeeded = new(true, string.Empty);
//    public static readonly AuthorisationResult Failed = new(false, string.Empty);

//    public bool IsAuthorized { get; }
//    public string FailureMessage { get; set; } = string.Empty;

//    public AuthorisationResult()
//    {
//    }

//    private AuthorisationResult(bool isAuthorized, string failureMessage)
//    {
//        IsAuthorized = isAuthorized;
//        FailureMessage = failureMessage;
//    }

//    public static AuthorisationResult Fail(string failureMessage) => new(false, failureMessage);
//}

