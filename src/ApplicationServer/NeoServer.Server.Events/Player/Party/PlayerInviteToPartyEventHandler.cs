﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Party;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Outgoing.Party;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player.Party;

public class PlayerInviteToPartyEventHandler
{
    private readonly IGameServer game;

    public PlayerInviteToPartyEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IPlayer leader, IPlayer invited, IParty party)
    {
        if (Guard.AnyNull(leader, invited, party)) return;

        if (!game.CreatureManager.GetPlayerConnection(leader.CreatureId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new PartyEmblemPacket(leader, PartyEmblem.Leader));
        connection.Send();
    }
}