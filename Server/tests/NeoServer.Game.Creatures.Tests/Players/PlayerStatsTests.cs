﻿using System.Collections.Generic;
using FluentAssertions;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Creatures.Player;
using NeoServer.Game.Tests.Helpers.Player;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players;

public class PlayerStatsTests
{
    [Theory]
    [InlineData(100, 50)]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    public void HasEnoughMana_ReturnsTrue(ushort mana, ushort required)
    {
        var sut = PlayerTestDataBuilder.Build(mana: mana);
        var result = sut.HasEnoughMana(required);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(100, 150)]
    [InlineData(0, 1)]
    public void HasEnoughMana_ReturnsFalse(ushort mana, ushort required)
    {
        var sut = PlayerTestDataBuilder.Build(mana: mana);
        var result = sut.HasEnoughMana(required);
        result.Should().BeFalse();
    }


    [Theory]
    [InlineData(100, 50)]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    public void HasEnoughLevel_ReturnsTrue(ushort level, ushort required)
    {
        var sut = PlayerTestDataBuilder.Build(skills: new Dictionary<SkillType, ISkill>
        {
            { SkillType.Level, new Skill(SkillType.Level, level) }
        });
        var result = sut.HasEnoughLevel(required);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(100, 150)]
    [InlineData(0, 1)]
    public void HasEnoughLevel_ReturnsFalse(ushort level, ushort required)
    {
        var sut = PlayerTestDataBuilder.Build(skills: new Dictionary<SkillType, ISkill>
        {
            { SkillType.Level, new Skill(SkillType.Level, level) }
        });
        var result = sut.HasEnoughLevel(required);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(100, 200, 100)]
    [InlineData(1, 1, 0)]
    [InlineData(0, 1, 1)]
    public void ConsumeMana_ChangeManaPoints(ushort consume, ushort mana, ushort expectedMana)
    {
        var sut = PlayerTestDataBuilder.Build(mana: mana);

        sut.ConsumeMana(consume);

        sut.Mana.Should().Be(expectedMana);
    }

    [Fact]
    public void ConsumeMana_MoreThanAvailable_DontChange()
    {
        var sut = PlayerTestDataBuilder.Build(mana: 200);

        sut.ConsumeMana(300);
        sut.Mana.Should().Be(200);
    }
}