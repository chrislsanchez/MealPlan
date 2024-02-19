using Serilog;

namespace MealPlanner.ViewModel;

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

        Ingredients.Add(new("Cinamon", Unit.grams, 150));        

    }

    #region Properties
    
    private RecipeDatabaseService _databaseService;
        
    // Create an ObservableCollection of Ingredient objects and initialize it with two ingredients
    public ObservableCollection<IngredientInfo> Ingredients { get; set; } = new ObservableCollection<IngredientInfo>
        {
            new ("Flour,", Unit.grams, 100),
            new("Sugar", Unit.grams,200),
            new("Salt", Unit.grams,300)
        };

    [ObservableProperty]
    List<string> ingredientList = new List<string>{ "1", "2", "3", "4" };
    [ObservableProperty]
    string addIngredientName;
    [ObservableProperty]
    uint addIngredientUnit;
    [ObservableProperty]
    float addIngredientQuantity;

    #endregion

    #region Commands

    [RelayCommand]
    void AddNewIngredient()
    {
        Ingredients.Add(new("Cinamon", Unit.grams, 150));
    }

    #endregion

}