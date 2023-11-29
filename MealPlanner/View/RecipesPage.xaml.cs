using MealPlanner.ViewModel;

namespace MealPlanner.View;

public partial class RecipesPage : ContentPage
{
	public RecipesPage(RecipesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}