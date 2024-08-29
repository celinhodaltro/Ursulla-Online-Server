﻿using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Common.Contracts.World.Tiles;

public interface ITileFactory
{
    ITile CreateTile(Coordinate coordinate, TileFlag flag, IItem[] items);
    ITile CreateDynamicTile(Coordinate coordinate, TileFlag flag, IItem[] items);
}