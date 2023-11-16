using MealPlanner.ViewModel;
namespace MealPlanner.View;

public partial class NewRecipePage : ContentPage
{
    public NewRecipePage(NewRecipeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}