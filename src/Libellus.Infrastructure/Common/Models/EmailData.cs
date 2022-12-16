using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Entities.Identity;

namespace Libellus.Infrastructure.Common.Models;

public sealed class EmailData : IEmailData
{
    public string UserEmail { get; init; }
    public string Username { get; init; }
    public string Subject { get; init; }
    public string Data { get; init; }

    public EmailData(User user, string subject, string data)
    {
        Guard.Against.Null(user);
        UserEmail = Guard.Against.NullOrWhiteSpace(user!.Email);
        Username = Guard.Against.NullOrWhiteSpace(user!.UserName);

        Subject = Guard.Against.NullOrWhiteSpace(subject);
        Data = Guard.Against.NullOrWhiteSpace(data);
    }
}