﻿using System.Collections.Generic;

namespace NeoServer.Game.Common.Contracts.DataStores;

public interface IDataStore
{
}

public interface IDataStore<TKey, TValue>
{
    IEnumerable<TValue> All { get; }
    IDictionary<TKey, TValue> Map { get; }
    void Add(TKey key, TValue value);
    TValue Get(TKey key);
    bool TryGetValue(TKey key, out TValue value);
    bool Contains(TKey key);
    void Clear();
}