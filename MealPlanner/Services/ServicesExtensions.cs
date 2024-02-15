using Serilog;
using System.Diagnostics;

namespace MealPlanner.Services;

public static class ServicesExtensions
{
    public static MauiAppBuilder ConfigureServices(this MauiAppBuilder builder)
    {
        //Logging.Configure(); //adding serilog logger

        Debug.Write("APP DEBUG: Aplication Directory : " + FileSystem.Current.AppDataDirectory);

        //Log.Verbose("Opening Database ... ");

        // Initialize the database service
        Debug.WriteLine("APP DEBUG:  Initialiting Database...");
        try
        {            
            RecipeDatabaseService dbService = new RecipeDatabaseService();
            Debug.WriteLine("APP DEBUG:  Succeeded.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("APP DEBUG: Database could not be open or created" + ex);
        }

        Debug.WriteLine("APP DEBUG:  Adding database service to builder");
        builder.Services.AddSingleton<RecipeDatabaseService>();

        // Services add 
        return builder;
    }
}
