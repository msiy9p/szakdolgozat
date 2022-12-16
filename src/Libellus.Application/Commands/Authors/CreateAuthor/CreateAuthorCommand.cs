using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Authors.CreateAuthor;

public sealed record CreateAuthorCommand(Name Name, CoverImageMetaDataContainer? CoverImageMetaDataContainer) :
    ICommand<AuthorIds>;