﻿using NeoServer.Game.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Models.Bases;

namespace NeoServer.Game.Creatures.Monster;

public abstract class WalkableMonster : CombatActor, IWalkableMonster
{
    protected WalkableMonster(ICreatureType type, IMapTool mapTool, IOutfit outfit = null, uint healthPoints = 0) :
        base(type, mapTool, outfit,
            healthPoints)
    {
    }

    public virtual IMonsterType Metadata => CreatureType as IMonsterType;
    public abstract bool CanAttackAnyTarget { get; }
    public override ITileEnterRule TileEnterRule => MonsterEnterTileRule.Rule;

    public bool LookForNewEnemy()
    {
        StopFollowing();
        StopAttack();

        Cooldowns.Start(CooldownType.Awaken, 10000);

        if (IsDead || CanAttackAnyTarget) return false;

        var direction = GetRandomStep();

        if (direction == Direction.None) return false;

        TryWalkTo(direction);

        return true;
    }

    internal void Escape(Location fromLocation)
    {
        StopFollowing();

        if (IsDead) return;
        if (MapTool?.PathFinder is null) return;

        var result = MapTool.PathFinder.Find(this, fromLocation, FindPathParams.EscapeParams, TileEnterRule);

        if (!result.Founded) return;

        TryWalkTo(result.Directions);
    }

    public void MoveAroundEnemy(CombatTarget enemy)
    {
        if (!Attacking) return;

        if (!Cooldowns.Expired(CooldownType.MoveAroundEnemy)) return;
        Cooldowns.Start(CooldownType.MoveAroundEnemy, GameRandom.Random.Next(3000, maxValue: 5000));

        var direction = GetRandomStep();
        if (direction == Direction.None) return;

        var nextLocation = Location.GetNextLocation(direction);

        var targetLocation = enemy.Creature.Location;

        var tooFar = targetLocation.GetMaxSqmDistance(nextLocation) > Metadata.MaxRangeDistanceAttack;

        if (Metadata.HasDistanceAttack && !enemy.CanReachCreature && enemy.HasSightClear && !tooFar)
        {
            TryWalkTo(direction);
            return;
        }

        if (targetLocation.GetMaxSqmDistance(nextLocation) > PathSearchParams.MaxTargetDist) return;
        TryWalkTo(direction);
    }
}