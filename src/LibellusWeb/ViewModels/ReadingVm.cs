using Libellus.Domain.Entities;

namespace LibellusWeb.ViewModels;

public sealed class ReadingVm
{
    public Reading Reading { get; init; }
    public string GroupId { get; init; }
    public string LinkBase { get; init; }
    public bool ShowEdit { get; init; }

    public ReadingVm(Reading reading, string groupId, string linkBase, bool showEdit)
    {
        Reading = reading;
        GroupId = groupId;
        LinkBase = linkBase;
        ShowEdit = showEdit;
    }
}