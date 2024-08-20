﻿using System;
using System.Collections.Generic;
using NeoServer.Data.InMemory.DataStores;
using NeoServer.Game.Chats;
using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Creatures.Services;

namespace NeoServer.Game.Tests.Helpers;

public static class PartyTestDataBuilder
{
    /// <summary>
    ///     Creates a party invite service and uses it to create a party from the provided players.
    /// </summary>
    /// <param name="partyInviteService">
    ///     Optional party invite service configured to your needs. If not provided, creates a
    ///     default one.
    /// </param>
    /// <param name="players"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns>The created party.</returns>
    public static IParty Build(IPartyInviteService partyInviteService, params IPlayer[] players)
    {
        if (players is not { Length: > 1 })
            throw new ArgumentOutOfRangeException(nameof(players),
                "Must provide at least two players to this helper to create a party.");

        partyInviteService ??= new PartyInviteService(new ChatChannelFactory
        {
            ChannelEventSubscribers = new List<IChatChannelEventSubscriber>(),
            ChatChannelStore = new ChatChannelStore()
        });

        var leader = players[0];
        for (var i = 1; i < players.Length; i++)
        {
            partyInviteService.Invite(leader, players[i]);
            players[i].PlayerParty.JoinParty(leader.PlayerParty.Party);
        }

        return leader.PlayerParty.Party;
    }
}