﻿using System;
using AutoFixture;
using FluentAssertions;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Creatures.Monster.Combat;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Monster;

public class MonsterYellTests
{
    [Fact]
    public void Monster_doesnt_yell_if_has_no_voices()
    {
        //arrange
        var monster = MonsterTestDataBuilder.Build();
        monster.Metadata.Voices = Array.Empty<Voice>();
        monster.Metadata.VoiceConfig = new IntervalChance(100, 50);
        using var monitor = monster.Monitor();

        //act
        monster.Yell();

        //assert
        monitor.Should().NotRaise(nameof(monster.OnSay));
    }

    [Fact]
    public void Monster_yells()
    {
        //arrange
        var sentence = new Fixture().Create<string>();

        var monster = MonsterTestDataBuilder.Build();
        monster.Metadata.Voices = new[] { new Voice(sentence, SpeechType.Say) };
        monster.Metadata.VoiceConfig = new IntervalChance(100, 100);

        using var monitor = monster.Monitor();

        //act
        monster.Yell();

        //assert
        monitor.Should().Raise(nameof(monster.OnSay));
    }
}