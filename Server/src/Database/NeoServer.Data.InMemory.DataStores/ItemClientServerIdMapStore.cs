﻿using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Data.InMemory.DataStores;

public class ItemClientServerIdMapStore : DataStore<ItemClientServerIdMapStore, ushort, ushort>,
    IItemClientServerIdMapStore
{
}