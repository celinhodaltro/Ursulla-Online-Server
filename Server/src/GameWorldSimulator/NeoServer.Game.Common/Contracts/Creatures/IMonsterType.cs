﻿using System.Collections.Generic;
using System.Collections.Immutable;
using NeoServer.Game.Common.Contracts.Combat;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures.Monsters;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Creatures;

public interface IMonsterType : ICreatureType
{
    ushort Armor { get; set; }
    ushort Defense { get; set; }

    public uint Experience { get; set; }
    public IMonsterCombatAttack[] Attacks { get; set; }
    public ICombatDefense[] Defenses { get; set; }

    IDictionary<CreatureFlagAttribute, ushort> Flags { get; set; }
    IIntervalChance TargetChance { get; set; }

    /// <summary>
    ///     Monster's phases
    /// </summary>
    Voice[] Voices { get; set; }

    /// <summary>
    ///     Voice interval and chance to happen
    /// </summary>
    IIntervalChance VoiceConfig { get; set; }

    ImmutableDictionary<DamageType, sbyte> ElementResistance { get; set; }
    Race Race { get; set; }
    ILoot Loot { get; set; }
    IMonsterSummon[] Summons { get; set; }
    byte MaxSummons { get; set; }
    ushort Immunities { get; set; }
    bool HasDistanceAttack { get; set; }
    byte MaxRangeDistanceAttack { get; set; }
    bool HasFlag(CreatureFlagAttribute flag);
}