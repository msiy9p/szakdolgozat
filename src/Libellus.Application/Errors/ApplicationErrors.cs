using Libellus.Domain.Common.Errors;

namespace Libellus.Application.Errors;

public static class ApplicationErrors
{
    public static class PaginationInfoErrors
    {
        public static readonly Error InvalidPageNumber =
            new(nameof(InvalidPageNumber), "Invalid page number.");

        public static readonly Error InvalidItemCount =
            new(nameof(InvalidItemCount), "Invalid item count.");
    }
}