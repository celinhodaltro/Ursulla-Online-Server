﻿using System;
using Moq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Server;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Managers;
using NeoServer.Server.Tasks;
using Serilog;

namespace NeoServer.Game.Tests.Server;

public static class GameServerTestBuilder
{
    public static IGameServer Build(IMap map)
    {
        var logger = new Mock<ILogger>().Object;
        var dispatcher = new Dispatcher(logger);

        var itemTypeStore = ItemTypeStoreTestBuilder.Build(Array.Empty<IItemType>());
        var decayableItemManager = DecayableItemManagerTestBuilder.Build(map, itemTypeStore);
        var persistenceDispatcher = new PersistenceDispatcher(logger);

        var gameServer = new GameServer(map, dispatcher, new OptimizedScheduler(dispatcher),
            new GameCreatureManager(new Mock<ICreatureGameInstance>().Object, map, logger), decayableItemManager,
            persistenceDispatcher);

        return gameServer;
    }
}