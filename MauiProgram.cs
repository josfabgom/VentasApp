using Microsoft.Extensions.Logging;
using VentasApp.Services;
using VentasApp.ViewModels;
using VentasApp.Views;
using CommunityToolkit.Maui;
using ZXing.Net.Maui.Controls;

namespace VentasApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseBarcodeReader()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Services
		builder.Services.AddSingleton<IDataService, MockDataService>();

		// ViewModels
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<ProductsViewModel>();
		builder.Services.AddTransient<ProductFormViewModel>();
		builder.Services.AddTransient<CustomersViewModel>();
		builder.Services.AddTransient<TakeOrderViewModel>();
		builder.Services.AddTransient<OrdersViewModel>();
		builder.Services.AddTransient<DeliveryViewModel>();
		builder.Services.AddTransient<VendedoresViewModel>();
		builder.Services.AddTransient<EquiposViewModel>();
		builder.Services.AddTransient<DashboardViewModel>();

		// Views
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<ProductsPage>();
		builder.Services.AddTransient<ProductFormPage>();
		builder.Services.AddTransient<CustomersPage>();
		builder.Services.AddTransient<TakeOrderPage>();
		builder.Services.AddTransient<OrdersPage>();
		builder.Services.AddTransient<DeliveryPage>();
		builder.Services.AddTransient<VendedoresPage>();
		builder.Services.AddTransient<EquiposPage>();
		builder.Services.AddTransient<DashboardPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
