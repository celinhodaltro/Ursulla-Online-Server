﻿using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;

namespace NeoServer.Game.Items.Services;

public class DecayService : IDecayService
{
    private readonly IItemTransformService _itemTransformService;

    public DecayService(IItemTransformService itemTransformService)
    {
        _itemTransformService = itemTransformService;
    }

    public void Decay(IItem item)
    {
        if (item.Decay is null) return;

        item.Decay.TryDecay();

        var result = _itemTransformService.Transform(item, item.Decay.DecaysTo);

        if (!result.Succeeded) return;

        result.Value?.Decay?.StartDecay();
    }
}