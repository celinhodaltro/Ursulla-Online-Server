﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Loaders.Interfaces;
using NeoServer.Server.Configurations;
using Newtonsoft.Json;
using Serilog;

namespace NeoServer.Loaders.Players.OutFits;

public class PlayerOutfitLoader : IStartupLoader
{
    private readonly ILogger _logger;
    private readonly IPlayerOutFitStore _playerOutFitStore;
    private readonly ServerConfiguration _serverConfiguration;

    public PlayerOutfitLoader(ServerConfiguration serverConfiguration, ILogger logger,
        IPlayerOutFitStore playerOutFitStore)
    {
        _serverConfiguration = serverConfiguration;
        _logger = logger;
        _playerOutFitStore = playerOutFitStore;
    }

    public void Load()
    {
        var path = $"{_serverConfiguration.Data}/player/outfits.json";

        if (!File.Exists(path))
        {
            _logger.Error("{Path} file not found", path);
            return;
        }

        var jsonContent = File.ReadAllText(path);
        var outfitsData = JsonConvert.DeserializeObject<IEnumerable<PlayerOutFitData>>(jsonContent).ToList();

        _playerOutFitStore.Add(Gender.Female, outfitsData.Where(item => item.Type == Gender.Female));
        _playerOutFitStore.Add(Gender.Male, outfitsData.Where(item => item.Type == Gender.Male));
    }
}