using MealPlanner.ViewModel;

namespace MealPlanner.View;

public partial class NewShoppingListPage : ContentPage
{
	public NewShoppingListPage(NewShoppingListViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}