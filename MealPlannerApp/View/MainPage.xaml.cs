using MealPlannerApp.ViewModel;

namespace MealPlannerApp;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();

        //BindingContext = new MainViewModel();        
        BindingContext = vm;
    }
}

