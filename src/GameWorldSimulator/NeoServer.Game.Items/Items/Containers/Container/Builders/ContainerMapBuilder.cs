﻿using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Game.Items.Items.Containers.Container.Builders;

internal static class ContainerMapBuilder
{
    public static IDictionary<ushort, uint> Build(IContainer container,
        Dictionary<ushort, uint> map = null)
    {
        map ??= new Dictionary<ushort, uint>();

        foreach (var item in container.Items)
        {
            if (map.TryGetValue(item.Metadata.TypeId, out var val)) map[item.Metadata.TypeId] = val + item.Amount;
            else map.Add(item.Metadata.TypeId, item.Amount);

            if (item is IContainer child) Build(child, map);
        }

        return map;
    }
}