namespace MealPlanner.ViewModel;
public partial class NewRecipeViewModel : BaseViewModel
{

    // Create an ObservableCollection of Ingredient objects and initialize it with two ingredients
    public ObservableCollection<Ingredient> Ingredients { get; set; } = new ObservableCollection<Ingredient>
        {
            new Ingredient { Name = "Flour", Unit = Unit.cups },
            new Ingredient { Name = "Sugar", Unit = Unit.grams },
            new Ingredient { Name = "Salt", Unit = Unit.grams }
        };

    public NewRecipeViewModel() { 
    
        Title = "Create new recipe";
    }


    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}