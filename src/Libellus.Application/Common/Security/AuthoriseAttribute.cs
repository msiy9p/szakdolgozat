namespace Libellus.Application.Common.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AuthoriseAttribute : Attribute
{
    public AuthoriseAttribute()
    {
    }

    public string Roles { get; set; } = string.Empty;

    public string Policy { get; set; } = string.Empty;

    public string GroupRoles { get; set; } = string.Empty;
}