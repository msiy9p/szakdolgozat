using FluentValidation;

namespace Libellus.Application.Commands.Books.UpdateBookCoverImageById;

public sealed class UpdateBookCoverImageByIdCommandValidator :
    AbstractValidator<UpdateBookCoverImageByIdCommand>
{
    public UpdateBookCoverImageByIdCommandValidator()
    {
        RuleFor(x => x.CoverImageMetaDataContainer)
            .NotNull();
    }
}