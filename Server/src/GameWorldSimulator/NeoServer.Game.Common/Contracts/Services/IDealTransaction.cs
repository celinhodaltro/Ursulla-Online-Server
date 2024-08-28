﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Common.Contracts.Services;

public interface IDealTransaction
{
    bool Buy(IPlayer buyer, IShopperNpc seller, IItemType itemType, byte amount);
}