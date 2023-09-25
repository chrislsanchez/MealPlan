using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MyTaskApp.ViewModel;


[QueryProperty("TextToPass", "TextToPass")]
public partial class DetailViewModel : ObservableObject
{
    [ObservableProperty]
    string textToPass;

    /// <summary>
    /// Asynchronous function for navigating back within a Shell-based application.
    /// Decorated with the [RelayCommand] attribute, indicating it can be invoked from a user interface element.
    /// When called, it navigates to the parent page using Shell navigation.
    /// </summary>    
    [RelayCommand]
    async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }


}
