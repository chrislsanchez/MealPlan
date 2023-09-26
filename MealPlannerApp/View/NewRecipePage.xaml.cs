using MealPlannerApp.ViewModel;

namespace MealPlannerApp.View;

public partial class NewRecipePage : ContentPage
{
    public NewRecipePage(NewRecipeViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}