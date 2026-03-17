using VentasApp.Views;

namespace VentasApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(ProductFormPage), typeof(ProductFormPage));
		Routing.RegisterRoute(nameof(VendedoresPage), typeof(VendedoresPage));
		Routing.RegisterRoute(nameof(EquiposPage), typeof(EquiposPage));
	}
}
