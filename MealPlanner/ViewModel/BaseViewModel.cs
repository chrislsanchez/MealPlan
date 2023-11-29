namespace MealPlanner.ViewModel;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title =  string.Empty;

    public bool IsNotBusy => !IsBusy;
}
