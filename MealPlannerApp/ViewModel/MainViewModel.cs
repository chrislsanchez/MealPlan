using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MealPlannerApp.View;
using System.Collections.ObjectModel;

namespace MealPlannerApp.ViewModel;

public partial class MainViewModel : ObservableObject
{

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


        Items.Add(MyTaskText);
        MyTaskText = string.Empty;
    }

    [RelayCommand]
    void Delete(string stringToDelete)
    {
        if (stringToDelete != null && Items.Contains(stringToDelete))
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
        //await Shell.Current.GoToAsync($"{nameof(DetailPage)}?TextToPass={stringToPass}");

        //Example passing complex data types
        //await Shell.Current.GoToAsync($"{nameof(DetailPage)}?Text={stringToPass}",
        //    new Dictionary<string, object>
        //    {
        //        {nameof(DetailPage),new object()},
        //    });

    }


    [RelayCommand]
    async Task AddNewRecipe()
    {
        await Shell.Current.GoToAsync(nameof(NewRecipePage));
    }


}
