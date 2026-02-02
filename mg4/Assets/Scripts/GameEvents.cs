using UnityEngine;
using System;


public static class GameEvents
{
    public static event Action<int> OnScoreIncreased;
    public static event Action OnPlayerDied;
    public static event Action OnPlayerJumped;

    public static void RaiseScoreIncreased(int addValue = 1)
    {
        OnScoreIncreased?.Invoke(addValue);
    }

    public static void RaisePlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

    public static void RaisePlayerJumped()
    {
        OnPlayerJumped?.Invoke();
    }
}