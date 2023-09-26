namespace MealPlannerApp.ViewModel;
public partial class NewRecipeViewModel : ObservableObject
{

    // Create an ObservableCollection of Ingredient objects and initialize it with two ingredients
    public ObservableCollection<Ingredient> Ingredients { get; set; } = new ObservableCollection<Ingredient>
        {
            new Ingredient { Text = "Flour", Unit = "Cups" },
            new Ingredient { Text = "Sugar", Unit = "Grams" },
            new Ingredient { Text = "Salt", Unit = "Grams" }
        };

    [ObservableProperty]
    string text;

    [RelayCommand]
    async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}

