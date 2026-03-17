using VentasApp.ViewModels;

namespace VentasApp.Views;

public partial class DashboardPage : ContentPage
{
	public DashboardPage(DashboardViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await (BindingContext as DashboardViewModel).LoadDashboardCommand.ExecuteAsync(null);
    }
}
