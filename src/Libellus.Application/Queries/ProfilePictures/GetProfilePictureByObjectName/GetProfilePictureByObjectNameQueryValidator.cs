using FluentValidation;

namespace Libellus.Application.Queries.ProfilePictures.GetProfilePictureByObjectName;

public sealed class GetProfilePictureByObjectNameQueryValidator : AbstractValidator<GetProfilePictureByObjectNameQuery>
{
    public GetProfilePictureByObjectNameQueryValidator()
    {
        RuleFor(x => x.ObjectName)
            .NotEmpty()
            .Length(1, 1024);
    }
}