using FluentValidation;

namespace Libellus.Application.Commands.Authors.UpdateAuthorCoverImageById;

public sealed class UpdateAuthorCoverImageByIdCommandValidator :
    AbstractValidator<UpdateAuthorCoverImageByIdCommand>
{
    public UpdateAuthorCoverImageByIdCommandValidator()
    {
        RuleFor(x => x.CoverImageMetaDataContainer)
            .NotNull();
    }
}