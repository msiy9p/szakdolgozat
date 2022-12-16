using FluentValidation;

namespace Libellus.Application.Commands.BookEditions.UpdateBookEditionCoverImageById;

public sealed class UpdateBookEditionCoverImageByIdCommandValidator :
    AbstractValidator<UpdateBookEditionCoverImageByIdCommand>
{
    public UpdateBookEditionCoverImageByIdCommandValidator()
    {
        RuleFor(x => x.CoverImageMetaDataContainer)
            .NotNull();
    }
}