﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Party;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Outgoing.Party;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player.Party;

public class PlayerInvitedToPartyEventHandler
{
    private readonly IGameServer game;

    public PlayerInvitedToPartyEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IPlayer leader, IPlayer invited, IParty party)
    {
        if (Guard.AnyNull(leader, invited, party)) return;

        if (game.CreatureManager.GetPlayerConnection(leader.CreatureId, out var leaderConnection))
        {
            leaderConnection.OutgoingPackets.Enqueue(new PartyEmblemPacket(invited, PartyEmblem.Invited));
            leaderConnection.Send();
        }

        if (game.CreatureManager.GetPlayerConnection(invited.CreatureId, out var invitedConnection))
        {
            invitedConnection.OutgoingPackets.Enqueue(new PartyEmblemPacket(leader, PartyEmblem.LeaderInvited));
            invitedConnection.Send();
        }
    }
}