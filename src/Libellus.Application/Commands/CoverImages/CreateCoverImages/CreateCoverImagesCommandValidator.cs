using FluentValidation;

namespace Libellus.Application.Commands.CoverImages.CreateCoverImages;

public sealed class CreateCoverImagesCommandValidator : AbstractValidator<CreateCoverImagesCommand>
{
    public CreateCoverImagesCommandValidator()
    {
        RuleFor(x => x.ImageDataOnly)
            .NotNull();
        RuleFor(x => x.ImageDataOnly.Data)
            .NotEmpty();
    }
}