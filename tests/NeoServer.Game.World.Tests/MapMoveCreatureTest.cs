﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Events;
using NeoServer.Game.Items;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Tests.Helpers.Map;
using NeoServer.Game.Tests.Helpers.Player;
using NeoServer.Game.World.Map;
using Xunit;

namespace NeoServer.Game.World.Tests;

public class MapMoveCreatureTest
{
    [Fact]
    public void TryMoveCreature_Should_Move_Creature()
    {
        var sut = MapTestDataBuilder.Build(1, 101, 1, 101, 6, 9);
        var player = PlayerTestDataBuilder.Build();
        player.SetNewLocation(new Location(50, 50, 7));
        sut.PlaceCreature(player);

        var result = sut.TryMoveCreature(player, new Location(51, 50, 7));

        Assert.True(result);
        Assert.Equal(new Location(51, 50, 7), player.Location);
    }

    [Fact]
    public void TryMoveCreature_when_Teleport_Should_Move_Creature()
    {
        var sut = MapTestDataBuilder.Build(1, 101, 1, 101, 6, 9);
        var player = PlayerTestDataBuilder.Build();

        player.SetNewLocation(new Location(50, 50, 7));
        sut.PlaceCreature(player);

        var result = sut.TryMoveCreature(player, new Location(53, 50, 7));

        Assert.True(result);
        Assert.Equal(new Location(53, 50, 7), player.Location);
    }

    [Fact]
    public void Player_dont_teleport_when_tile_has_teleport_without_destination()
    {
        //arrange

        var sut = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 7);
        var pathFinder = new PathFinder(sut);

        var player = PlayerTestDataBuilder.Build(pathFinder: pathFinder);

        player.SetCurrentTile((IDynamicTile)sut[100, 100, 7]);
        sut.PlaceCreature(player);

        var teleportLocation = new Location(101, 100, 7);

        var teleportAttrs = new Dictionary<ItemAttribute, IConvertible>
        {
            //no destination
        };

        ((IDynamicTile)sut[teleportLocation]).AddItem(new TeleportItem(new ItemType(), teleportLocation,
            teleportAttrs));

        player.OnStartedWalking += c => sut.MoveCreature(c);

        //act
        player.WalkTo(Direction.East);

        //assert
        player.Location.X.Should().Be(101);
        player.Location.Y.Should().Be(100);
        player.Location.Z.Should().Be(7);
    }

    [Fact]
    public void Player_teleports_when_tile_has_teleport_with_a_destination()
    {
        //arrange
        var teleportLocation = new Location(101, 100, 7);
        var teleportAttrs = new Dictionary<ItemAttribute, IConvertible>
        {
            [ItemAttribute.TeleportDestination] = new Location(105, 105, 7)
        };

        var teleport = new TeleportItem(new ItemType(), teleportLocation, teleportAttrs);

        var sut = MapTestDataBuilder.Build(100, 105, 100, 105, 7, 7, true,
            new Dictionary<Location, IItem[]>
            {
                [teleportLocation] = new IItem[] { teleport }
            });
        var pathFinder = new PathFinder(sut);

        var player = PlayerTestDataBuilder.Build(pathFinder: pathFinder);
        player.SetCurrentTile((IDynamicTile)sut[100, 100, 7]);
        sut.PlaceCreature(player);

        player.OnStartedWalking += c => sut.MoveCreature(c);
        player.OnTeleported += (a, b) => new CreatureTeleportedEventHandler(sut).Execute(a, b);

        //act
        player.WalkTo(Direction.East);

        //assert
        player.Location.X.Should().Be(105);
        player.Location.Y.Should().Be(105);
        player.Location.Z.Should().Be(7);
    }
}