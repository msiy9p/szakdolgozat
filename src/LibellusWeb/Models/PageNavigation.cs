using Libellus.Application.Models;

namespace LibellusWeb.Models;

public sealed class PageNavigation
{
    private const string Previous = "Previous";
    private const string Next = "Next";
    private const string First = "First";
    private const string Last = "Last";

    public string DisplayText { get; init; }
    public string Url { get; init; }
    public bool IsCurrent { get; init; }
    public bool IsDisabled { get; init; }

    public PageNavigation(string displayText, string url, bool isCurrent, bool isDisabled)
    {
        DisplayText = displayText;
        Url = url;
        IsCurrent = isCurrent;
        IsDisabled = isDisabled;
    }

    public static PageNavigation CreateCurrent(string displayText) =>
        new(displayText, string.Empty, true, false);

    public static PageNavigation CreateDisabled(string displayText) =>
        new(displayText, string.Empty, false, true);

    public static PageNavigation CreateClickable(string displayText, string url) =>
        new(displayText, url, false, false);

    public static List<PageNavigation> CreateEmpty()
    {
        var output = new List<PageNavigation>
        {
            CreateDisabled(First),
            CreateDisabled(Previous),
            CreateCurrent("1"),
            CreateDisabled(Next),
            CreateDisabled(Last)
        };

        return output;
    }

    public static List<PageNavigation> CreateNavigations(PaginationDetail pagination, string baseUrl, int chunkSize = 5)
    {
        var output = new List<PageNavigation>();

        if (pagination.TotalPages <= 1)
        {
            output.Add(CreateDisabled(First));
            output.Add(CreateDisabled(Previous));
            output.Add(CreateCurrent("1"));
            output.Add(CreateDisabled(Next));
            output.Add(CreateDisabled(Last));

            return output;
        }

        // First
        if (pagination.CurrentPageNumber == 1)
        {
            output.Add(CreateDisabled(First));
        }
        else
        {
            output.Add(CreateClickable(First, CreateUrl(baseUrl, pagination.GetFirstPage()!.Value)));
        }

        // Previous
        if (pagination.HasPreviousPage)
        {
            output.Add(CreateClickable(Previous, CreateUrl(baseUrl, pagination.GetPreviousPage()!.Value)));
        }
        else
        {
            output.Add(CreateDisabled(Previous));
        }

        // Numbered in chunk
        foreach (var item in Chunkify(pagination.CurrentPageNumber, pagination.TotalPages, chunkSize))
        {
            if (pagination.CurrentPageNumber == item)
            {
                output.Add(CreateCurrent(item.ToString()));
            }
            else
            {
                output.Add(CreateClickable(item.ToString(), CreateUrl(baseUrl, pagination.GetPage(item)!.Value)));
            }
        }

        // Next
        if (pagination.HasNextPage)
        {
            output.Add(CreateClickable(Next, CreateUrl(baseUrl, pagination.GetNextPage()!.Value)));
        }
        else
        {
            output.Add(CreateDisabled(Next));
        }

        // Last
        if (pagination.CurrentPageNumber == pagination.TotalPages)
        {
            output.Add(CreateDisabled(Last));
        }
        else
        {
            output.Add(CreateClickable(Last, CreateUrl(baseUrl, pagination.GetLastPage()!.Value)));
        }

        return output;
    }

    private static IEnumerable<int> Chunkify(int current, int total, int chunkSize)
    {
        var first = (current - 1) / chunkSize;
        first = (first * chunkSize) + 1;

        var last = first + chunkSize - 1;

        if (total < last)
        {
            return Enumerable.Range(first, chunkSize - (last - total));
        }

        return Enumerable.Range(first, chunkSize);
    }

    private static string CreateUrl(string baseUrl, PaginationInfo pagination)
        => $"{baseUrl}/{(int)pagination.ItemCount}/{pagination.PageNumber}";
}