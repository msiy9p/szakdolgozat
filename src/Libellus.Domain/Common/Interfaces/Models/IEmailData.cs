namespace Libellus.Domain.Common.Interfaces.Models;

public interface IEmailData
{
    string UserEmail { get; }
    string Username { get; }
    string Subject { get; }
    string Data { get; }
}