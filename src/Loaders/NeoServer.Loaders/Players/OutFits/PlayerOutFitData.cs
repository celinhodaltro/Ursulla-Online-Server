﻿using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using Newtonsoft.Json;

namespace NeoServer.Loaders.Players.OutFits;

[Serializable]
public class PlayerOutFitData : IPlayerOutFit
{
    [JsonProperty("type")] public Gender Type { get; init; }
    [JsonProperty("looktype")] public ushort LookType { get; init; }
    [JsonProperty("name")] public string Name { get; init; }
    [JsonProperty("premium")] public bool RequiresPremium { get; init; }
    [JsonProperty("unlocked")] public bool Unlocked { get; init; }
    [JsonProperty("enabled")] public bool Enabled { get; init; }
}