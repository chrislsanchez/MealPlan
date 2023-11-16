namespace MealPlanner.ViewModel;
public partial class NewRecipeViewModel : ObservableObject
{

    // Create an ObservableCollection of Ingredient objects and initialize it with two ingredients
    public ObservableCollection<Ingredient> Ingredients { get; set; } = new ObservableCollection<Ingredient>
        {
            new Ingredient { Name = "Flour", Unit = Unit.cups },
            new Ingredient { Name = "Sugar", Unit = Unit.grams },
            new Ingredient { Name = "Salt", Unit = Unit.grams }
        };

    [ObservableProperty]
    string text;

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}