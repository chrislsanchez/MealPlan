using MealPlanner.View;
namespace MealPlanner
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register the page on the route
            Routing.RegisterRoute(nameof(NewRecipePage), typeof(NewRecipePage)); // this gives a name and type of detail page to navigate to
        }
    }
}
