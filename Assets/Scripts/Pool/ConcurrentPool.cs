using System;
using System.Collections.Concurrent;

public sealed class ConcurrentPool
{
    private readonly ConcurrentDictionary<Type, ConcurrentStack<IResettable>> concurrentPool =
        new ConcurrentDictionary<Type, ConcurrentStack<IResettable>>();

    public ResettableType Get<ResettableType>(params object[] parameters) where ResettableType : IResettable
    {
        Type resettableType = typeof(ResettableType);
        if (!concurrentPool.ContainsKey(resettableType))
        {
            concurrentPool.TryAdd(resettableType, new ConcurrentStack<IResettable>());
        }

        ResettableType val;
        if (concurrentPool[resettableType].Count > 0)
        {
            concurrentPool[resettableType].TryPop(out IResettable resettable);
            val = (ResettableType)resettable;
        }
        else
        {
            val = (ResettableType)Activator.CreateInstance(resettableType);
        }

        val.Assign(parameters);
        return val;
    }

    public void Release<ResetteableType>(ResetteableType resettable) where ResetteableType : IResettable
    {
        resettable.Reset();
        concurrentPool[typeof(ResetteableType)].Push(resettable);
    }
}
