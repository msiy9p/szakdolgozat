using Libellus.Domain.Entities;

namespace LibellusWeb.ViewModels;

public sealed class GroupVm
{
    public Group Group { get; init; }

    public GroupVm(Group group)
    {
        Group = group;
    }
}