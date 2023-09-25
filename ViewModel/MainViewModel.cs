using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MyTaskApp.ViewModel;

public partial class MainViewModel : ObservableObject
{

    IConnectivity connectivity;
    
    public MainViewModel(IConnectivity connectivity) 
    {
        Items = new ObservableCollection<string>();  
        this.connectivity = connectivity;
    }




    [ObservableProperty]
    ObservableCollection<string> items;

    [ObservableProperty]
    string myTaskText;

    [RelayCommand]
    async void Add()
    {
        if (string.IsNullOrEmpty(MyTaskText)) 
        {
            return;
        }

        if(connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("No Internet", "Please check your internet connection to continue", "OK");
            return;
        }


        Items.Add(MyTaskText);
        MyTaskText = string.Empty;
    }

    [RelayCommand]
    void Delete(string stringToDelete)
    {
        if(stringToDelete != null && Items.Contains(stringToDelete))
        {
            Items.Remove(stringToDelete);
        }
    }

    /// <summary>
    /// Asynchronous function for handling a tap event with a parameter.
    /// When called, it navigates to the 'DetailPage' using Shell navigation and includes a query parameter 'Text' with the provided 'stringToPass' value.
    /// The 'stringToPass' parameter is used to pass additional information to the 'DetailPage'.
    /// </summary>
    /// <param name="stringToPass"></param>
    /// <returns></returns>
    [RelayCommand]
    async Task Tap(string stringToPass)
    {
        await Shell.Current.GoToAsync($"{nameof(DetailPage)}?TextToPass={stringToPass}");

        //Example passing complex data types
        //await Shell.Current.GoToAsync($"{nameof(DetailPage)}?Text={stringToPass}",
        //    new Dictionary<string, object>
        //    {
        //        {nameof(DetailPage),new object()},
        //    });

    }


}
