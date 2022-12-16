using FluentValidation;

namespace Libellus.Application.Commands.ProfilePictures.CreateProfilePictures;

public sealed class CreateProfilePicturesCommandValidator : AbstractValidator<CreateProfilePicturesCommand>
{
    public CreateProfilePicturesCommandValidator()
    {
        RuleFor(x => x.ImageDataOnly)
            .NotNull();
        RuleFor(x => x.ImageDataOnly.Data)
            .NotEmpty();
    }
}