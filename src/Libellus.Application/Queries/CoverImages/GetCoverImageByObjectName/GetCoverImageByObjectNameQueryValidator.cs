using FluentValidation;

namespace Libellus.Application.Queries.CoverImages.GetCoverImageByObjectName;

public sealed class GetCoverImageByObjectNameQueryValidator : AbstractValidator<GetCoverImageByObjectNameQuery>
{
    public GetCoverImageByObjectNameQueryValidator()
    {
        RuleFor(x => x.ObjectName)
            .NotEmpty()
            .Length(1, 1024);
    }
}