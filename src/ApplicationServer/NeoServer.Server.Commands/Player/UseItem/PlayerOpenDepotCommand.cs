using System.Linq;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Parsers;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Systems.Depot;
using NeoServer.Networking.Packets.Incoming;

namespace NeoServer.Server.Commands.Player.UseItem;

public class PlayerOpenDepotCommand
{
    private readonly DepotManager _depotManager;
    private readonly IItemFactory _itemFactory;
    private readonly IPlayerDepotItemRepository _playerDepotItemRepository;
    private readonly IPlayerUseService _playerUseService;

    public PlayerOpenDepotCommand(IPlayerUseService playerUseService,
        IPlayerDepotItemRepository playerDepotItemRepository, IItemFactory itemFactory, DepotManager depotManager)
    {
        _playerUseService = playerUseService;
        _playerDepotItemRepository = playerDepotItemRepository;
        _itemFactory = itemFactory;
        _depotManager = depotManager;
    }

    public void Execute(IPlayer player, IDepot depot, UseItemPacket useItemPacket)
    {
        var playerDepot = LoadDepot(player, depot);

        playerDepot.SetNewLocation(useItemPacket.Location);

        _playerUseService.Use(player, playerDepot, useItemPacket.Index);
    }

    private IDepot LoadDepot(IPlayer player, IItem container)
    {
        var depot = _depotManager.Get(player.Id);
        if (depot is not null) return depot;

        var depotRecordsTask = _playerDepotItemRepository.GetByPlayerId(player.Id);

        depot = (IDepot)_itemFactory.Create(container.Metadata, container.Location, null);

        var depotRecords = depotRecordsTask.Result.ToList();

        var depotItemModels = depotRecords.ToList();

        ItemEntityParser.BuildContainer(depot, depotItemModels, container.Location, _itemFactory);

        _depotManager.Load(player.Id, depot);

        return depot;
    }
}