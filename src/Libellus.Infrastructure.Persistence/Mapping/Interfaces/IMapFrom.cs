namespace Libellus.Infrastructure.Persistence.Mapping.Interfaces;

internal interface IMapFrom<in TItem, out TOut>
{
    static abstract TOut Map(TItem item1);
}

internal interface IMapFrom<in TItem1, in TItem2, out TOut>
{
    static abstract TOut Map(TItem1 item1, TItem2 item2);
}

internal interface IMapFrom<in TItem1, in TItem2, in TItem3, out TOut>
{
    static abstract TOut Map(TItem1 item1, TItem2 item2, TItem3 item3);
}

internal interface IMapFrom<in TItem1, in TItem2, in TItem3, in TItem4, out TOut>
{
    static abstract TOut Map(TItem1 item1, TItem2 item2, TItem3 item3, TItem4 item4);
}

internal interface IMapFrom<in TItem1, in TItem2, in TItem3, in TItem4, in TItem5, out TOut>
{
    static abstract TOut Map(TItem1 item1, TItem2 item2, TItem3 item3, TItem4 item4, TItem5 item5);
}

internal interface IMapFrom<in TItem1, in TItem2, in TItem3, in TItem4, in TItem5, in TItem6, out TOut>
{
    static abstract TOut Map(TItem1 item1, TItem2 item2, TItem3 item3, TItem4 item4, TItem5 item5, TItem6 item6);
}

internal interface IMapFrom<in TItem1, in TItem2, in TItem3, in TItem4, in TItem5, in TItem6, in TItem7, out TOut>
{
    static abstract TOut Map(TItem1 item1, TItem2 item2, TItem3 item3, TItem4 item4, TItem5 item5, TItem6 item6,
        TItem7 item7);
}

internal interface IMapFrom<in TItem1, in TItem2, in TItem3, in TItem4, in TItem5, in TItem6, in TItem7, in TItem8,
    out TOut>
{
    static abstract TOut Map(TItem1 item1, TItem2 item2, TItem3 item3, TItem4 item4, TItem5 item5, TItem6 item6,
        TItem7 item7, TItem8 item8);
}