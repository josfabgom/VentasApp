using VentasApp.ViewModels;

namespace VentasApp.Views;

public partial class VendedoresPage : ContentPage
{
	public VendedoresPage(VendedoresViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await (BindingContext as VendedoresViewModel).LoadVendedoresCommand.ExecuteAsync(null);
    }
}
