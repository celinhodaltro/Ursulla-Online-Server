﻿using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using NeoServer.Game.Chats;
using NeoServer.Game.Chats.Rules;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Creatures.Player;
using NeoServer.Game.Tests.Helpers.Player;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players;

public class PlayerChannelTests
{
    [Fact]
    public void Player_adds_personal_channel()
    {
        //arrange
        var sut = PlayerTestDataBuilder.Build();
        var chatChannel = new ChatChannel(1, "my channel");
        //act
        sut.Channels.AddPersonalChannel(chatChannel);

        //arrange
        sut.Channels.PersonalChannels.Should().Contain(chatChannel);
    }

    [Fact]
    public void Player_cannot_add_an_invalid_personal_channel()
    {
        //arrange
        var sut = PlayerTestDataBuilder.Build();
        //act
        sut.Channels.AddPersonalChannel(null);

        //arrange
        sut.Channels.PersonalChannels.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Player_cannot_join_invalid_channel()
    {
        //arrange
        var sut = PlayerTestDataBuilder.Build();
        using var monitor = sut.Channels.Monitor();
        //act
        var result = sut.Channels.JoinChannel(null);

        //arrange
        result.Should().BeFalse();
        sut.Channels.PersonalChannels.Should().BeNullOrEmpty();
        sut.Channels.PrivateChannels.Should().BeNullOrEmpty();

        monitor.Should().NotRaise(nameof(sut.Channels.OnJoinedChannel));
    }

    [Fact]
    public void Player_cannot_join_channel_when_already_in()
    {
        //arrange
        var sut = PlayerTestDataBuilder.Build();
        using var sutMonitor = sut.Channels.Monitor();

        var channel = new ChatChannel(1, "channel 1");
        channel.AddUser(sut);
        //act
        var result = sut.Channels.JoinChannel(channel);

        //arrange
        result.Should().BeFalse();
        sut.Channels.PersonalChannels.Should().BeNullOrEmpty();
        sut.Channels.PrivateChannels.Should().BeNullOrEmpty();

        sutMonitor.Should().NotRaise(nameof(sut.Channels.OnJoinedChannel));
    }

    [Fact]
    public void Player_cannot_join_channel_with_unmeeting_rules()
    {
        //arrange
        var sut = PlayerTestDataBuilder.Build(skills: new Dictionary<SkillType, ISkill>
        {
            [SkillType.Level] = new Skill(SkillType.Level, 1)
        });

        using var sutMonitor = sut.Channels.Monitor();

        var channel = new ChatChannel(1, "channel 1")
        {
            JoinRule = new ChannelRule
            {
                MinMaxAllowedLevel = (100, 200)
            }
        };

        //act
        var result = sut.Channels.JoinChannel(channel);

        //arrange
        result.Should().BeFalse();
        sut.Channels.PersonalChannels.Should().BeNullOrEmpty();
        sut.Channels.PrivateChannels.Should().BeNullOrEmpty();

        sutMonitor.Should().NotRaise(nameof(sut.Channels.OnJoinedChannel));
    }

    [Fact]
    public void Player_joins_channel()
    {
        //arrange
        var sut = PlayerTestDataBuilder.Build(skills: new Dictionary<SkillType, ISkill>
        {
            [SkillType.Level] = new Skill(SkillType.Level, 101)
        });

        using var sutMonitor = sut.Channels.Monitor();

        var channel = new ChatChannel(1, "channel 1")
        {
            JoinRule = new ChannelRule
            {
                MinMaxAllowedLevel = (100, 200)
            }
        };

        //act
        var result = sut.Channels.JoinChannel(channel);

        //arrange
        result.Should().BeTrue();
        sut.Channels.PersonalChannels.Should().BeNullOrEmpty();
        sut.Channels.PrivateChannels.Should().BeNullOrEmpty();

        sutMonitor.Should().Raise(nameof(sut.Channels.OnJoinedChannel));
    }

    [Fact]
    public void Private_channels_return_both_guild_and_party_channels()
    {
        //arrange
        var guild = new Guild.Guild();
        guild.Channel = new GuildChatChannel(1, "guild channel, guild", guild);

        var sut = PlayerTestDataBuilder.Build(guild: guild);
        var partyFriend = PlayerTestDataBuilder.Build();

        var partyChannel = new ChatChannel(1, "party channel");
        var party = new Party.Party(partyFriend, partyChannel);
        partyFriend.PlayerParty.InviteToParty(sut, party);

        sut.PlayerParty.JoinParty(party);

        //act
        var result = sut.Channels.PrivateChannels;

        //arrange
        result.Should().HaveCount(2);
        result.Should().Contain(guild.Channel);
        result.Should().Contain(party.Channel);
    }

    #region Exit channels tests

    [Fact]
    public void Player_cannot_exit_invalid_channel()
    {
        //arrange
        var sut = PlayerTestDataBuilder.Build();
        using var monitor = sut.Channels.Monitor();

        //act
        var result = sut.Channels.ExitChannel(null);

        //arrange
        result.Should().BeFalse();

        monitor.Should().NotRaise(nameof(sut.Channels.OnExitedChannel));
    }

    [Fact]
    public void Player_cannot_exit_channel_that_is_not_in()
    {
        //arrange
        var sut = PlayerTestDataBuilder.Build();
        using var sutMonitor = sut.Channels.Monitor();

        var channel = new ChatChannel(1, "channel 1");
        //act
        var result = sut.Channels.ExitChannel(channel);

        //arrange
        result.Should().BeFalse();

        sutMonitor.Should().NotRaise(nameof(sut.Channels.OnExitedChannel));
    }

    [Fact]
    public void Player_exits_channel()
    {
        //arrange
        var sut = PlayerTestDataBuilder.Build();
        using var monitor = sut.Channels.Monitor();

        var channel = new ChatChannel(1, "channel 1");
        sut.Channels.JoinChannel(channel);

        //act
        var result = sut.Channels.ExitChannel(channel);

        //arrange
        result.Should().BeTrue();
        monitor.Should().Raise(nameof(sut.Channels.OnExitedChannel));
    }

    #endregion

    #region Send message to channel tests

    [Fact]
    public void Player_cannot_send_message_to_channel_that_is_not_in()
    {
        //arrange
        var sut = PlayerTestDataBuilder.Build();

        var channel = new ChatChannel(1, "channel 1");
        var message = new Fixture().Create<string>();
        //act
        var result = sut.Channels.SendMessage(channel, message);

        //arrange
        result.Should().BeFalse();
    }

    [Fact]
    public void Player_sends_message_to_channel()
    {
        //arrange
        var sut = PlayerTestDataBuilder.Build();

        var channel = new ChatChannel(1, "channel 1");
        sut.Channels.JoinChannel(channel);

        var message = new Fixture().Create<string>();
        //act
        var result = sut.Channels.SendMessage(channel, message);

        //arrange
        result.Should().BeTrue();
    }

    #endregion
}