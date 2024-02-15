using Serilog;
using System.Diagnostics;

namespace MealPlanner.ViewModel;
public partial class NewRecipeViewModel : BaseViewModel
{

    public NewRecipeViewModel(RecipeDatabaseService databaseService)
    {
        _databaseService = databaseService;

        Debug.WriteLine("APP DEBUG: In New Recepie View Model Constructor. Database Path = " + _databaseService.DatabasePath);

    }

    private RecipeDatabaseService? _databaseService;


    // Create an ObservableCollection of Ingredient objects and initialize it with two ingredients
    public ObservableCollection<Ingredient> Ingredients { get; set; } = new ObservableCollection<Ingredient>
        {
            new Ingredient { Name = "Flour", Unit = Unit.cups },
            new Ingredient { Name = "Sugar", Unit = Unit.grams },
            new Ingredient { Name = "Salt", Unit = Unit.grams }
        };

    public NewRecipeViewModel() {     

        Title = "Create new recipe";
        //_dbService = dbService;

        //if (_dbService == null)
        //{
        //    Debug.WriteLine("The dependency injection did not work");
        //}
        //else
        //{
        //    Debug.WriteLine("The dependency injection Works");
        //}

    }


}