using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoServer.Data.Contexts;
using NeoServer.Data.Entities;
using NeoServer.Data.Parsers;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Data.Repositories.Player;

public static class ContainerManager
{
    public static async Task Save<TPlayerItemEntity>(IPlayer player, IContainer container, NeoContext neoContext)
        where TPlayerItemEntity : PlayerItemBaseEntity, new()
    {
        if (Guard.AnyNull(player, container)) return;

        if (container?.Items?.Count == 0) return;

        var containerId = 0;
        var containers = new Queue<(IContainer Container, int ParentId)>();
        containers.Enqueue((container, containerId));

        while (containers.TryDequeue(out var dequeuedContainer))
        {
            var items = dequeuedContainer.Container.Items;
            if (!items.Any()) continue;

            foreach (var item in items)
            {
                var itemModel = ItemEntityParser.ToPlayerItemEntity<TPlayerItemEntity>(item);
                if (itemModel is null) continue;

                itemModel.PlayerId = (int)player.Id;
                itemModel.ParentId = dequeuedContainer.ParentId;

                if (item is IContainer innerContainer)
                {
                    itemModel.ContainerId = ++containerId;
                    containers.Enqueue((innerContainer, itemModel.ContainerId));
                }

                await neoContext.AddAsync(itemModel);
            }
        }
    }
}