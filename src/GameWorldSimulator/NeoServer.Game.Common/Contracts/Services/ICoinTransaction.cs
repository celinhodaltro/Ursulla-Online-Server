﻿using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Services;

public interface ICoinTransaction
{
    void AddCoins(IPlayer player, ulong amount);
    ulong RemoveCoins(IPlayer player, ulong amount, out ulong change);
    bool RemoveCoins(IPlayer player, ulong amount, bool useBank = false);
}