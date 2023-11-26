using Serilog;

namespace MealPlanner.Services;

public static class Logging
{
    public static void Configure()
    {
        string logDir = Path.Combine(FileSystem.Current.AppDataDirectory, "mealPlanLog.txt");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(logDir, rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}
