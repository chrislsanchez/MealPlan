using Serilog;
using System.Diagnostics;

namespace MealPlanner.Services;

public static class Logging
{
    public static void Configure()
    {
        Debug.Write("Log Directory : " + FileSystem.Current.AppDataDirectory);
        string logDir = Path.Combine(FileSystem.Current.AppDataDirectory, "mealPlanLog.txt");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(logDir, rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}
