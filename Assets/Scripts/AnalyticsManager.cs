using Firebase.Analytics;

public static class AnalyticsManager
{
    private static void LogEvent(string eventName, params Parameter[] parameters)
    {
        FirebaseAnalytics.LogEvent(eventName, parameters);
    }

    public static void LogUpgradeEvent(int resourceIndex, int level)
    {
        LogEvent(
            FirebaseAnalytics.EventLevelUp, 
            new Parameter(FirebaseAnalytics.ParameterIndex, resourceIndex.ToString()),
            new Parameter(FirebaseAnalytics.ParameterLevel, level)
            );
    }

    public static void LogUnlockEvent(int resourceIndex)
    {
        LogEvent(
            FirebaseAnalytics.EventUnlockAchievement,
            new Parameter(FirebaseAnalytics.ParameterIndex, resourceIndex.ToString())
            );
    }

    public static void SetUserProperties(string name, string value)
    {
        FirebaseAnalytics.SetUserProperty(name, value);
    }
}