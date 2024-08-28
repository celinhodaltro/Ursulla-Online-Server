﻿using FluentAssertions;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.Tests.Helpers.Map;
using NeoServer.Game.Tests.Helpers.Player;
using NeoServer.Game.World.Models.Tiles;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Monster;

public class MonsterTest
{
    [Fact]
    public void Monster_is_not_injured_when_attacked_by_another_monster()
    {
        //arrange
        var map = MapTestDataBuilder.Build(100, 102, 100, 102, 7, 7);

        var sut = MonsterTestDataBuilder.Build();
        var attacker = MonsterTestDataBuilder.Build();

        (map[100, 100, 7] as DynamicTile)?.AddCreature(sut);
        (map[101, 100, 7] as DynamicTile)?.AddCreature(attacker);

        using var monitor = sut.Monitor();

        //act
        attacker.Attack(sut);

        //assert
        sut.HealthPoints.Should().Be(sut.MaxHealthPoints);
        monitor.Should().NotRaise(nameof(sut.OnAttacked));
        monitor.Should().NotRaise(nameof(sut.OnInjured));
    }

    [Fact]
    public void Monster_is_not_injured_when_attacked_by_a_summon_of_a_monster()
    {
        //arrange
        var map = MapTestDataBuilder.Build(100, 102, 100, 102, 7, 7);

        var sut = MonsterTestDataBuilder.Build();

        var master = MonsterTestDataBuilder.Build();
        var summon = MonsterTestDataBuilder.BuildSummon(master);

        (map[100, 100, 7] as DynamicTile)?.AddCreature(sut);
        (map[101, 100, 7] as DynamicTile)?.AddCreature(master);
        (map[100, 101, 7] as DynamicTile)?.AddCreature(summon);

        using var monitor = sut.Monitor();

        //act
        summon.Attack(sut);

        //assert
        sut.HealthPoints.Should().Be(sut.MaxHealthPoints);
        monitor.Should().NotRaise(nameof(sut.OnAttacked));
        monitor.Should().NotRaise(nameof(sut.OnInjured));
    }

    [Fact]
    public void Monster_is_injured_when_attacked_by_a_summon_of_a_player()
    {
        //arrange

        var map = MapTestDataBuilder.Build(100, 102, 100, 102, 7, 7);

        var sut = MonsterTestDataBuilder.Build(9000);

        var master = PlayerTestDataBuilder.Build();
        var summon = MonsterTestDataBuilder.BuildSummon(master, 4000, 5000);

        (map[100, 100, 7] as DynamicTile)?.AddCreature(sut);
        (map[101, 100, 7] as DynamicTile)?.AddCreature(master);
        (map[100, 101, 7] as DynamicTile)?.AddCreature(summon);

        //act
        summon.Attack(sut);

        //assert
        sut.HealthPoints.Should().BeLessThan(sut.MaxHealthPoints);
    }
}