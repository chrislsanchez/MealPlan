﻿using Microsoft.Extensions.Logging;
using MealPlanner.View;
using MealPlanner.ViewModel;
using System.Diagnostics;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Debug.Write("\nAPP DIRECTORY : " + FileSystem.Current.AppDataDirectory + "\n");

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureServices()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });            

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainViewModel>();

            builder.Services.AddTransient<NewRecipePage>();
            builder.Services.AddTransient<NewRecipeViewModel>();

            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddTransient<AboutViewModel>();

            builder.Services.AddTransient<CalenderPage>();
            builder.Services.AddTransient<CalenderViewModel>();

            builder.Services.AddTransient<GroceryListsPage>();
            builder.Services.AddTransient<GroceryListsViewModel>();

            builder.Services.AddTransient<NewShoppingListPage>();
            builder.Services.AddTransient<NewShoppingListViewModel>();

            builder.Services.AddTransient<RecipesPage>();
            builder.Services.AddTransient<RecipesViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
