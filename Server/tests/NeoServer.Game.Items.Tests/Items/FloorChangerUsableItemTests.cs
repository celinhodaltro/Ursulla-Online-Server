﻿using System.Collections.Generic;
using FluentAssertions;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Events;
using NeoServer.Game.Items.Items.UsableItems;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.Tests.Helpers.Map;
using NeoServer.Game.Tests.Helpers.Player;
using NeoServer.Game.World.Models.Tiles;
using Xunit;

namespace NeoServer.Game.Items.Tests.Items;

public class FloorChangerUsableItemTests
{
    [Fact]
    public void Player_is_sent_to_another_floor_when_uses_a_floor_changer_item()
    {
        //arrange
        var backpack = ItemTestData.CreateBackpack();

        var floorChangerItemType = new ItemType();
        floorChangerItemType.SetOnUse();
        floorChangerItemType.OnUse.SetAttribute(ItemAttribute.UseOn, new long[] { 100, 101, 102 });
        floorChangerItemType.OnUse.SetAttribute(ItemAttribute.FloorChange, "up");

        floorChangerItemType.Flags.Add(ItemFlag.Pickupable);
        floorChangerItemType.Flags.Add(ItemFlag.Movable);

        var floorChangerItem = new FloorChangerUsableItem(floorChangerItemType, Location.Zero);

        var location = new Location(100, 100, 7);
        var ground = MapTestDataBuilder.CreateGround(location, 100);
        var tile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, ground, null, null);
        var aboveTile = new DynamicTile(new Coordinate(101, 100, 6), TileFlag.None, ground, null, null);

        var map = MapTestDataBuilder.Build(tile, aboveTile);

        backpack.AddItem(floorChangerItem);
        var player = PlayerTestDataBuilder.Build(inventoryMap: new Dictionary<Slot, (IItem Item, ushort Id)>
        {
            [Slot.Backpack] = new(backpack, 1)
        });
        player.OnTeleported += new CreatureTeleportedEventHandler(map).Execute;

        tile.AddCreature(player);

        //act
        player.Use(floorChangerItem, tile);

        //assert
        player.Location.Z.Should().Be(6);
    }

    [Fact]
    public void Player_is_not_sent_to_another_floor_if_tile_has_no_rope_spot()
    {
        //arrange
        var backpack = ItemTestData.CreateBackpack();

        var floorChangerItemType = new ItemType();
        floorChangerItemType.SetOnUse();
        floorChangerItemType.OnUse.SetAttribute(ItemAttribute.UseOn, new long[] { 200, 201, 202 });
        floorChangerItemType.OnUse.SetAttribute(ItemAttribute.FloorChange, "up");
        var floorChangerItem = new FloorChangerUsableItem(floorChangerItemType, Location.Zero);

        var location = new Location(100, 100, 7);
        var ground = MapTestDataBuilder.CreateGround(location, 100);
        var tile = new DynamicTile(new Coordinate(100, 100, 7), TileFlag.None, ground, null, null);
        var aboveTile = new DynamicTile(new Coordinate(101, 100, 6), TileFlag.None, ground, null, null);

        var map = MapTestDataBuilder.Build(tile, aboveTile);

        backpack.AddItem(floorChangerItem);
        var player = PlayerTestDataBuilder.Build(inventoryMap: new Dictionary<Slot, (IItem Item, ushort Id)>
        {
            [Slot.Backpack] = new(backpack, 1)
        });
        player.OnTeleported += new CreatureTeleportedEventHandler(map).Execute;

        tile.AddCreature(player);

        //act
        player.Use(floorChangerItem, tile);

        //assert
        player.Location.Z.Should().Be(7);
    }
}