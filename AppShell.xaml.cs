namespace MyTaskApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        // Registers a route for the DetailPage, associating it with the DetailPage class.
        // This allows the application's routing system to navigate to the DetailPage when the route is matched.
        Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));

	}
}
