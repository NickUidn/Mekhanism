using System;

public static class GameEvents
{
    public static event Action NewCycleStartedAction;
    public static void NewCycleStarted() => NewCycleStartedAction?.Invoke();
}
