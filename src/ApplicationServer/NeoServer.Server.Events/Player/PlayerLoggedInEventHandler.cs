﻿using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player;

public class PlayerLoggedInEventHandler : IEventHandler
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerLoggedInEventHandler(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async void Execute(IWalkableCreature creature)
    {
        if (creature.IsNull()) return;

        if (creature is not IPlayer player) return;

        await _playerRepository.UpdatePlayerOnlineStatus(player.Id, true);
    }
}