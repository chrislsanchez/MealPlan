using Serilog;

namespace MealPlanner.ViewModel;

public partial class DisplayedIngredient() : ObservableObject
{
    [ObservableProperty]
    string? name;
    [ObservableProperty]
    Unit unit;
    [ObservableProperty]
    float quantity;

    public DisplayedIngredient(string name, Unit unit, float quantity) : this()
    {
        Name = name;
        Unit = unit;
        Quantity = quantity;
    }

}

public partial class NewRecipeViewModel : BaseViewModel
{




    public NewRecipeViewModel(RecipeDatabaseService databaseService)
    {
        Title = "Create new recipe";

        Log.Debug("New Recipe view model called");
        _databaseService = databaseService;

        if (databaseService == null)
        {
            Log.Error("The database Service is null on NewRecipeViewModel");
        }

        DisplayedIngredients.Add(new("Cinamon", Unit.grams, 150));

    }

    private RecipeDatabaseService _databaseService;

    // Create an ObservableCollection of Ingredient objects and initialize it with two ingredients
    public ObservableCollection<DisplayedIngredient> DisplayedIngredients { get; set; } = new ObservableCollection<DisplayedIngredient>
        {
            new ("Flour,", Unit.grams, 100),
            new("Sugar", Unit.grams,200),
            new("Salt", Unit.grams,300)
        };

    [ObservableProperty]
    List<string> ingredientList = new List<string>{ "1", "2" };

    [RelayCommand]
    void AddNewIngredient()
    {
        DisplayedIngredients.Add(new("Cinamon", Unit.grams, 150));
    }

}