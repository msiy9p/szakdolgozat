namespace Libellus.Application.Enums;

public static class PaginationItemCountExtensions
{
    public static readonly PaginationItemCount Biggest = (PaginationItemCount)GetAll().Cast<int>().Max();
    public static readonly PaginationItemCount Smallest = (PaginationItemCount)GetAll().Cast<int>().Min();

    public static bool IsDefined(PaginationItemCount paginationItemCount) =>
        paginationItemCount switch
        {
            PaginationItemCount.Items5 => true,
            PaginationItemCount.Items10 => true,
            PaginationItemCount.Items25 => true,
            PaginationItemCount.Items50 => true,
            PaginationItemCount.Items100 => true,
            _ => false
        };

    public static IEnumerable<PaginationItemCount> GetAll()
    {
        yield return PaginationItemCount.Items5;
        yield return PaginationItemCount.Items10;
        yield return PaginationItemCount.Items25;
        yield return PaginationItemCount.Items50;
        yield return PaginationItemCount.Items100;
    }
}