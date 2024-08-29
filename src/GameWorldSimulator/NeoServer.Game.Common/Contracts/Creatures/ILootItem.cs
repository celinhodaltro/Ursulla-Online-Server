﻿using System;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Common.Contracts.Creatures;

public interface ILootItem
{
    Func<IItemType> ItemType { get; }
    byte Amount { get; }
    uint Chance { get; }
    ILootItem[] Items { get; }
}