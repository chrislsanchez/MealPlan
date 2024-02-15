using MealPlanner.View;
using Serilog;

namespace MealPlanner.ViewModel;

public partial class MainViewModel : BaseViewModel
{

    //public MainViewModel(RecipeDatabaseService recipesDatabase)
    //{
    //    _recipesDatabase = recipesDatabase;
    //}

    private RecipeDatabaseService _recipesDatabase;

    [ObservableProperty]
    ObservableCollection<string>? items;

    [ObservableProperty]
    string? myTaskText;

    [ObservableProperty]    
    private string? _databasePathText = "Here will the database path be inserted";

    [RelayCommand]
    Task<Task> Add()
    {
        if (string.IsNullOrEmpty(MyTaskText))
        {
            return Task.FromResult(Task.CompletedTask);
        }

        if (Items is not null)
        {
            Items.Add(MyTaskText);
        }
        MyTaskText = string.Empty;
        return Task.FromResult(Task.CompletedTask);
    }

    [RelayCommand]
    void Delete(string stringToDelete)
    {
        if (Items is null)
        {
            return;
        }

        if (stringToDelete != null && Items.Contains(stringToDelete))
        {
            Items.Remove(stringToDelete);
        }
    }

    [RelayCommand]
    async Task AddNewRecipe()
    {
        Log.Debug("Adding New Recipe Page initialized");
        await Shell.Current.GoToAsync(nameof(NewRecipePage));
    }

}

