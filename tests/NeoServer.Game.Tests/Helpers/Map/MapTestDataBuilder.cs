﻿using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items;
using NeoServer.Game.Items.Items;
using NeoServer.Game.World.Models.Tiles;

namespace NeoServer.Game.Tests.Helpers.Map;

public static class MapTestDataBuilder
{
    public static IMap Build(params ITile[] tiles)
    {
        var world = new World.World();
        var map = new World.Map.Map(world);

        foreach (var tile in tiles) world.AddTile(tile);

        return map;
    }

    public static IMap Build(params Func<ITile>[] tiles)
    {
        var world = new World.World();
        var map = new World.Map.Map(world);

        foreach (var tile in tiles) world.AddTile(tile?.Invoke());

        return map;
    }

    public static IMap Build(int fromX, int toX, int fromY, int toY, int fromZ, int toZ, bool addGround = true,
        IDictionary<Location, IItem[]> topItems = null,
        List<Location> staticTiles = null)
    {
        topItems ??= new Dictionary<Location, IItem[]>();
        staticTiles ??= new List<Location>();

        var world = new World.World();
        var map = new World.Map.Map(world);

        for (var x = fromX; x <= toX; x++)
        for (var y = fromY; y <= toY; y++)
        for (var z = fromZ; z <= toZ; z++)
        {
            IGround ground = null;

            var location = new Location((ushort)x, (ushort)y, (byte)z);

            if (addGround) ground = new Ground(new ItemType(), new Location((ushort)x, (ushort)y, (byte)z));

            topItems.TryGetValue(location, out var items);

            if (staticTiles.Contains(location)) world.AddTile(new StaticTile(new Coordinate(x, y, (sbyte)z)));

            world.AddTile(new DynamicTile(new Coordinate(x, y, (sbyte)z), TileFlag.None, ground,
                items ?? Array.Empty<IItem>(), null));
        }

        return map;
    }

    public static Ground CreateGround(Location location, ushort id = 1, int speed = 50)
    {
        var itemType = new ItemType();
        itemType.SetId(id);
        itemType.Attributes?.SetAttribute(ItemAttribute.Speed, speed);

        return new Ground(itemType, location);
    }

    public static IDynamicTile CreateTile(Location location, ushort id = 1, int speed = 50)
    {
        var random = new Random();
        var ground = CreateGround(location, (ushort)random.Next(1, ushort.MaxValue));

        return new DynamicTile(new Coordinate(location), TileFlag.None, ground, Array.Empty<IItem>(),
            Array.Empty<IItem>());
    }

    public static IDynamicTile CreateTile(Location location, ushort id = 1, int speed = 50, params IItem[] downItems)
    {
        var random = new Random();
        var ground = CreateGround(location, (ushort)random.Next(1, ushort.MaxValue));

        return new DynamicTile(new Coordinate(location), TileFlag.None, ground, Array.Empty<IItem>(),
            downItems);
    }
}