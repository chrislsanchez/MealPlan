using MealPlanner.ViewModel;

namespace MealPlanner.View;

public partial class CalenderPage : ContentPage
{
	public CalenderPage(CalenderViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}