using MealPlanner.ViewModel;

namespace MealPlanner;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();

        //BindingContext = new MainViewModel();        
        BindingContext = vm;
    }
}