﻿using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Spells;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Combat.Defenses;

public class HasteCombatDefense : BaseCombatDefense
{
    public HasteCombatDefense(uint duration, ushort speedBoost, EffectT effect)
    {
        Spell = new HasteSpell(duration, speedBoost, effect);
    }

    public ISpell Spell { get; }

    public override void Defend(ICombatActor actor)
    {
        Spell?.Invoke(actor, null, out var error);
    }
}