﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Common.Contracts.Services;

public interface IItemTransformService
{
    Result<IItem> Transform(IPlayer by, IItem fromItem, ushort toItem);
    Result<IItem> Transform(IItem fromItem, ushort toItem);
}