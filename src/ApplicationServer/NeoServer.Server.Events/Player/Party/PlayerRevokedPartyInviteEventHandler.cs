﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Party;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Networking.Packets.Outgoing.Party;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player.Party;

public class PlayerRevokedPartyInviteEventHandler
{
    private readonly IGameServer game;

    public PlayerRevokedPartyInviteEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IPlayer by, IPlayer invited, IParty party)
    {
        if (Guard.AnyNull(by, invited)) return;

        foreach (var spectator in game.Map.GetPlayersAtPositionZone(invited.Location))
        {
            if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

            if (Equals(spectator, invited))
                connection.OutgoingPackets.Enqueue(
                    new TextMessagePacket($"{by.Name} has revoked her invitation",
                        TextMessageOutgoingType.Small)); //todo set correct gender

            if (Equals(spectator, by))
                connection.OutgoingPackets.Enqueue(new TextMessagePacket(
                    $"Invitation for {invited.Name} has been revoked",
                    TextMessageOutgoingType.Small)); //todo set correct gender

            connection.OutgoingPackets.Enqueue(new PartyEmblemPacket(invited, PartyEmblem.None));
            connection.Send();
        }
    }
}