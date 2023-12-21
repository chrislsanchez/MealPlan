namespace MealPlanner.ViewModel;

public partial class AboutViewModel : BaseViewModel
{
    public AboutViewModel() 
    {
        Title = "About this App";
    }

    [ObservableProperty]
    private string _aboutText = "This application is public. Any changes and commercialisation are subject to the licence agreement.";


}
