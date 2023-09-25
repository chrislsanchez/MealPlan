using MyTaskApp.ViewModel;

namespace MyTaskApp;

public partial class MainPage : ContentPage
{	
	public MainPage(MainViewModel vm)
	{
		InitializeComponent();

        //BindingContext = new MainViewModel();        
        BindingContext = vm;
	}

}

