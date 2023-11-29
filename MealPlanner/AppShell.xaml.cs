using MealPlanner.View;
namespace MealPlanner
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register the routes
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
            Routing.RegisterRoute(nameof(CalenderPage), typeof(CalenderPage));
            Routing.RegisterRoute(nameof(GroceriesPage), typeof(GroceriesPage));
            Routing.RegisterRoute(nameof(GroceryListsPage), typeof(GroceryListsPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(NewRecipePage), typeof(NewRecipePage)); // this gives a name and type of detail page to navigate to
            Routing.RegisterRoute(nameof(NewShoppingListPage), typeof(NewShoppingListPage));
            Routing.RegisterRoute(nameof(RecipesPage), typeof(RecipesPage));
        }
    }
}
