using MealPlanner.ViewModel;

namespace MealPlanner.View;

public partial class GroceriesPage : ContentPage
{
	public GroceriesPage(GroceriesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}