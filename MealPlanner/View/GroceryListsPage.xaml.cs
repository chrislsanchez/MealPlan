using MealPlanner.ViewModel;

namespace MealPlanner.View;

public partial class GroceryListsPage : ContentPage
{
	public GroceryListsPage(GroceriesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}