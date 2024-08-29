﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Outgoing.Item;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Tiles;

public class ThingAddedToTileEventHandler
{
    private readonly IGameServer game;

    public ThingAddedToTileEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IThing thing, ICylinder cylinder)
    {
        if (Guard.AnyNull(cylinder, cylinder.TileSpectators, thing)) return;
        var tile = cylinder.ToTile;
        if (tile.IsNull()) return;

        var spectators = cylinder.TileSpectators;

        foreach (var spectator in spectators)
        {
            if (!game.CreatureManager.GetPlayerConnection(spectator.Spectator.CreatureId, out var connection))
                continue;

            if (spectator.Spectator is not IPlayer player) continue;

            if (!player.CanSee(thing.Location)) continue;

            connection.OutgoingPackets.Enqueue(new AddTileItemPacket((IItem)thing, spectator.ToStackPosition));

            connection.Send();
        }
    }
}